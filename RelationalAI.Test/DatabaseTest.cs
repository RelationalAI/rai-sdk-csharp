using System;
using System.Threading.Tasks;
using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace RelationalAI.Test
{
    public class DatabaseTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{UUID}";
        public static string EngineName = $"csharp-sdk-{UUID}";
        [Fact]
        public async Task DatabaseTest()
        {
            Client client = CreateClient();
            await client.CreateEngineWaitAsync(EngineName, size: EngineSize.XS);

            await Assert.ThrowsAsync<SystemException>(async () => await client.DeleteDatabaseAsync(Dbname) );

            var createRsp = await client.CreateDatabaseAsync(Dbname, EngineName, overwrite: false);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            createRsp = await client.CreateDatabaseAsync(Dbname, EngineName, overwrite: true);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            var database = await client.GetDatabaseAsync(Dbname);
            Assert.Equal(Dbname, database.Name);
            Assert.Equal("CREATED", database.State);

            var databases = await client.ListDatabasesAsync();
            database = databases.Find( db => db.Name == Dbname );
            Assert.Equal(Dbname, database.Name);
            Assert.Equal("CREATED", database.State);

            databases = await client.ListDatabasesAsync("CREATED");
            database = databases.Find( db => db.Name == Dbname );
            Assert.Equal(Dbname, database.Name);
            Assert.Equal("CREATED", database.State);

            databases = await client.ListDatabasesAsync("NONESENSE");
            Assert.Empty(databases);

            var edbs = await client.ListEdbsAsync(Dbname, EngineName);
            var edb = edbs.Find( item => item.Name.Equals("rel"));
            Assert.NotNull(edb);

            var modelNames = await client.ListModelNamesAsync(Dbname, EngineName);
            var name = modelNames.Find( item => item.Equals("rel/stdlib") );
            Assert.NotNull(name);

            var models = await client.ListModelsAsync(Dbname, EngineName);
            var model = models.Find( m => m.Name.Equals("rel/stdlib") );
            Assert.NotNull(model);

            model = await client.GetModelAsync(Dbname, EngineName, "rel/stdlib");
            Assert.NotNull(model);
            Assert.True(model.Value.Length > 0);

            var deleteRsp = await client.DeleteDatabaseAsync(Dbname);
            Assert.Equal(Dbname, deleteRsp.Name);

            await Assert.ThrowsAsync<SystemException>(async () => await client.GetDatabaseAsync(Dbname) );
        }

        readonly string testModel =
                "def R = \"hello\", \"world\"";
        readonly string testJson = "{" +
                "\"name\":\"Amira\",\n" +
                "\"age\":32,\n" +
                "\"height\":null,\n" +
                "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public async Task DatabaseCloneTest()
        {
            Client client = CreateClient();
            await client.CreateEngineWaitAsync(EngineName, size: EngineSize.XS);

            await Assert.ThrowsAsync<SystemException>(async () => await client.DeleteDatabaseAsync(Dbname) );

            // create a fresh database
            var createRsp = await client.CreateDatabaseAsync(Dbname, EngineName);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            // load some data and model
            var loadRsp = await client.LoadJsonAsync(Dbname, EngineName, "test_data", testJson);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            loadRsp = await client.LoadModelAsync(Dbname, EngineName, "test_model", testModel);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            // clone database
            var databaseCloneName = $"{Dbname}-clone";
            createRsp = await client.CloneDatabaseAsync(databaseCloneName, EngineName, Dbname, true);
            Assert.Equal(databaseCloneName, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            // make sure the database exists
            var database = await client.GetDatabaseAsync(databaseCloneName);
            Assert.Equal(databaseCloneName, database.Name);
            Assert.Equal("CREATED", database.State);

            var databases = await client.ListDatabasesAsync();
            database = databases.Find( db => db.Name == databaseCloneName );
            Assert.NotNull(database);
            Assert.Equal(databaseCloneName, database.Name);
            Assert.Equal("CREATED", database.State);

            // make sure the data was cloned
            var rsp = await client.ExecuteV1Async(databaseCloneName, EngineName, "test_data", true);

            Relation rel;

            rel = findRelation(rsp.Output, ":name");
            Assert.NotNull(rel);

            rel = findRelation(rsp.Output, ":age");
            Assert.NotNull(rel);

            rel = findRelation(rsp.Output, ":height");
            Assert.NotNull(rel);

            rel = findRelation(rsp.Output, ":pets");
            Assert.NotNull(rel);

            // make sure the model was cloned
            var modelNames = await client.ListModelNamesAsync(databaseCloneName, EngineName);
            var name = modelNames.Find( item => item.Equals("test_model") );
            Assert.NotNull(name);

            var models = await client.ListModelsAsync(databaseCloneName, EngineName);
            var model = models.Find( m => m.Name.Equals("test_model") );
            Assert.NotNull(model);

            model = await client.GetModelAsync(databaseCloneName, EngineName, "test_model");
            Assert.NotNull(model);
            Assert.True(model.Value.Length > 0);

            // cleanup
            var deleteRsp = await client.DeleteDatabaseAsync(databaseCloneName);
            Assert.Equal(databaseCloneName, deleteRsp.Name);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteDatabaseAsync(Dbname); } catch {}
            try { await client.DeleteEngineWaitAsync(EngineName); } catch {}
        }
    }
}
