using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class DatabaseTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        public static string EngineName = $"csharp-sdk-{Uuid}";

        [Fact]
        public async Task DatabaseTest()
        {
            var client = CreateClient();
            var engine = await CreateEngineWaitAsync(EngineName);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.DeleteDatabaseAsync(Dbname));

            var createRsp = await client.CreateDatabaseAsync(Dbname, engine.Name, false);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal(DatabaseState.Created, createRsp.State);

            createRsp = await client.CreateDatabaseAsync(Dbname, engine.Name, true);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal(DatabaseState.Created, createRsp.State);

            var database = await client.GetDatabaseAsync(Dbname);
            Assert.Equal(Dbname, database.Name);
            Assert.Equal(DatabaseState.Created, database.State);

            var databases = await client.ListDatabasesAsync();
            database = databases.Find(db => db.Name == Dbname);
            Assert.Equal(Dbname, database.Name);
            Assert.Equal(DatabaseState.Created, database.State);

            databases = await client.ListDatabasesAsync(DatabaseState.Created);
            database = databases.Find(db => db.Name == Dbname);
            Assert.Equal(Dbname, database.Name);
            Assert.Equal(DatabaseState.Created, database.State);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                client.ListDatabasesAsync((DatabaseState)1000));

            var edbs = await client.ListEdbsAsync(Dbname, engine.Name);
            var edb = edbs.Find(item => item.Name.Equals("rel"));
            Assert.NotNull(edb);

            var modelNames = await client.ListModelsAsync(Dbname, engine.Name);
            var name = modelNames.Find(item => item.Equals("rel/stdlib"));
            Assert.NotNull(name);

            var models = await client.ListModelsAsync(Dbname, engine.Name);

            var deleteRsp = await client.DeleteDatabaseAsync(Dbname);
            Assert.Equal(Dbname, deleteRsp.Name);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.GetDatabaseAsync(Dbname));
        }

        private readonly Dictionary<string, string> TestModel = new Dictionary<string, string> { { "test_model", "def R = \"hello\", \"world\"" } };

        private const string TestJson = "{" +
                                        "\"name\":\"Amira\",\n" +
                                        "\"age\":32,\n" +
                                        "\"height\":null,\n" +
                                        "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public async Task DatabaseCloneTest()
        {
            Environment.SetEnvironmentVariable("ENGINE_NAME", "csharp-sdk-98daf872-dff1-4624-960f-f69123cb0715");
            var client = CreateClient();
            var engine = await CreateEngineWaitAsync(EngineName);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.DeleteDatabaseAsync(Dbname));

            // create a fresh database
            var createRsp = await client.CreateDatabaseAsync(Dbname, engine.Name);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal(DatabaseState.Created, createRsp.State);

            // load some data and model
            var loadRsp = await client.LoadJsonAsync(Dbname, engine.Name, "test_data", TestJson);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var resp = await client.LoadModelsWaitAsync(Dbname, engine.Name, TestModel);
            Assert.Equal(TransactionAsyncState.Completed, resp.Transaction.State);
            Assert.Empty(resp.Problems);

            // clone database
            var databaseCloneName = $"{Dbname}-clone";
            createRsp = await client.CloneDatabaseAsync(databaseCloneName, engine.Name, Dbname, true);
            Assert.Equal(databaseCloneName, createRsp.Name);
            Assert.Equal(DatabaseState.Created, createRsp.State);

            // make sure the database exists
            var database = await client.GetDatabaseAsync(databaseCloneName);
            Assert.Equal(databaseCloneName, database.Name);
            Assert.Equal(DatabaseState.Created, database.State);

            var databases = await client.ListDatabasesAsync();
            database = databases.Find(db => db.Name == databaseCloneName);
            Assert.NotNull(database);
            Assert.Equal(databaseCloneName, database.Name);
            Assert.Equal(DatabaseState.Created, database.State);

            // make sure the data was cloned
            var rsp = await client.ExecuteV1Async(databaseCloneName, engine.Name, "test_data", true);

            var rel = FindRelation(rsp.Output, ":name");
            Assert.NotNull(rel);

            rel = FindRelation(rsp.Output, ":age");
            Assert.NotNull(rel);

            rel = FindRelation(rsp.Output, ":height");
            Assert.NotNull(rel);

            rel = FindRelation(rsp.Output, ":pets");
            Assert.NotNull(rel);

            // make sure the model was cloned
            var modelNames = await client.ListModelsAsync(databaseCloneName, engine.Name);
            var name = modelNames.Find(item => item.Equals("test_model"));
            Assert.NotNull(name);

            var model = await client.GetModelAsync(databaseCloneName, engine.Name, "test_model");
            Assert.Equal("test_model", model.Name);
            Assert.Equal(TestModel["test_model"], model.Value);

            // cleanup
            var deleteRsp = await client.DeleteDatabaseAsync(databaseCloneName);
            Assert.Equal(databaseCloneName, deleteRsp.Name);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try
            {
                await client.DeleteDatabaseAsync(Dbname);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }

            try
            {
                DeleteEngineWaitAsync(EngineName);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
