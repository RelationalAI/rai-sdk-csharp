using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class DatabaseTests : UnitTest
    {
        private readonly EngineFixture engineFixture;
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";

        public DatabaseTests(EngineFixture fixture)
        {
            engineFixture = fixture;
        }

        [Fact]
        public async Task DatabaseTest()
        {
            var client = CreateClient();

            await engineFixture.CreateEngineWaitAsync();

            await client
                .Invoking(c => c.DeleteDatabaseAsync(Dbname))
                .Should().ThrowAsync<HttpError>();

            var createRsp = await client.CreateDatabaseAsync(Dbname, engineFixture.Engine.Name, false);
            createRsp.Name.Should().Be(Dbname);
            createRsp.State.Should().Be(DatabaseState.Created);

            createRsp = await client.CreateDatabaseAsync(Dbname, engineFixture.Engine.Name, true);
            createRsp.Name.Should().Be(Dbname);
            createRsp.State.Should().Be(DatabaseState.Created);

            var database = await client.GetDatabaseAsync(Dbname);
            database.Name.Should().Be(Dbname);
            database.State.Should().Be(DatabaseState.Created);

            var databases = await client.ListDatabasesAsync();
            database = databases.Find(db => db.Name == Dbname);
            database.Name.Should().Be(Dbname);
            database.State.Should().Be(DatabaseState.Created);

            databases = await client.ListDatabasesAsync(DatabaseState.Created);
            database = databases.Find(db => db.Name == Dbname);
            database.Name.Should().Be(Dbname);
            database.State.Should().Be(DatabaseState.Created);

            await client
                .Invoking(c => c.ListDatabasesAsync((DatabaseState)1000))
                .Should().ThrowAsync<ArgumentOutOfRangeException>();

            var edbs = await client.ListEdbsAsync(Dbname, engineFixture.Engine.Name);
            var edb = edbs.Find(item => item.Name.Equals("rel"));
            edb.Should().NotBeNull();

            var modelNames = await client.ListModelsAsync(Dbname, engineFixture.Engine.Name);
            var name = modelNames.Find(item => item.Equals("rel/stdlib"));
            name.Should().NotBeNull();

            var models = await client.ListModelsAsync(Dbname, engineFixture.Engine.Name);

            var deleteRsp = await client.DeleteDatabaseAsync(Dbname);
            deleteRsp.Name.Should().Be(Dbname);

            await client
                .Invoking(c => c.GetDatabaseAsync(Dbname))
                .Should().ThrowAsync<HttpError>();
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
            var client = CreateClient();

            await engineFixture.CreateEngineWaitAsync();

            await client
                .Invoking(c => c.DeleteDatabaseAsync(Dbname))
                .Should().ThrowAsync<HttpError>();

            // create a fresh database
            var createRsp = await client.CreateDatabaseAsync(Dbname, engineFixture.Engine.Name);
            createRsp.Name.Should().Be(Dbname);
            createRsp.State.Should().Be(DatabaseState.Created);

            // load some data and model
            var loadRsp = await client.LoadJsonAsync(Dbname, engineFixture.Engine.Name, "test_data", TestJson);
            loadRsp.Aborted.Should().BeFalse();
            loadRsp.Output.Should().HaveCount(0);
            loadRsp.Problems.Should().HaveCount(0);

            var resp = await client.LoadModelsWaitAsync(Dbname, engineFixture.Engine.Name, TestModel);
            resp.Transaction.State.Should().Be(TransactionAsyncState.Completed);
            resp.Problems.Should().HaveCount(0);

            // clone database
            var databaseCloneName = $"{Dbname}-clone";
            createRsp = await client.CloneDatabaseAsync(databaseCloneName, engineFixture.Engine.Name, Dbname, true);
            createRsp.Name.Should().Be(databaseCloneName);
            createRsp.State.Should().Be(DatabaseState.Created);

            // make sure the database exists
            var database = await client.GetDatabaseAsync(databaseCloneName);
            database.Name.Should().Be(databaseCloneName);
            database.State.Should().Be(DatabaseState.Created);

            var databases = await client.ListDatabasesAsync();
            database = databases.Find(db => db.Name == databaseCloneName);
            database.Should().NotBeNull();
            database.Name.Should().Be(databaseCloneName);
            database.State.Should().Be(DatabaseState.Created);

            // make sure the data was cloned
            var rsp = await client.ExecuteV1Async(databaseCloneName, engineFixture.Engine.Name, "test_data", true);

            var rel = FindRelation(rsp.Output, ":name");
            rel.Should().NotBeNull();

            rel = FindRelation(rsp.Output, ":age");
            rel.Should().NotBeNull();

            rel = FindRelation(rsp.Output, ":height");
            rel.Should().NotBeNull();

            rel = FindRelation(rsp.Output, ":pets");
            rel.Should().NotBeNull();

            // make sure the model was cloned
            var modelNames = await client.ListModelsAsync(databaseCloneName, engineFixture.Engine.Name);
            var name = modelNames.Find(item => item.Equals("test_model"));
            name.Should().NotBeNull();

            var model = await client.GetModelAsync(databaseCloneName, engineFixture.Engine.Name, "test_model");
            model.Name.Should().Be("test_model");
            model.Value.Should().Be(TestModel["test_model"]);

            // cleanup
            var deleteRsp = await client.DeleteDatabaseAsync(databaseCloneName);
            deleteRsp.Name.Should().Be(databaseCloneName);
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
        }
    }
}
