using System;
using Xunit;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;


namespace RelationalAI.Test
{
    public class DatabaseTests : UnitTest
    {
        [Fact]
        public void DatabaseTest()
        {
            Client client = CreateClient();
            client.CreateEngineWait(EngineName, size: EngineSize.XS);

            var rsp = client.DeleteTransaction(Dbname);
            Assert.Equal("Not Found", (JsonConvert.DeserializeObject(rsp) as JObject).GetValue("status").ToString());

            var createRsp = client.CreateDatabase(Dbname, EngineName, overwrite: false);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            createRsp = client.CreateDatabase(Dbname, EngineName, overwrite: true);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            var database = client.GetDatabase(Dbname);
            Assert.Equal(Dbname, database.Name);
            Assert.Equal("CREATED", database.State);

            var databases = client.ListDatabases();
            database = databases.Find( db => db.Name == Dbname );
            Assert.Equal(Dbname, database.Name);
            Assert.Equal("CREATED", database.State);

            databases = client.ListDatabases("CREATED");
            database = databases.Find( db => db.Name == Dbname );
            Assert.Equal(Dbname, database.Name);
            Assert.Equal("CREATED", database.State);

            databases = client.ListDatabases("NONESENSE");
            Assert.Empty(databases);

            var edbs = client.ListEdbs(Dbname, EngineName);
            var edb = edbs.Find( item => item.Name.Equals("rel"));
            Assert.NotNull(edb);

            var modelNames = client.ListModelNames(Dbname, EngineName);
            var name = modelNames.Find( item => item.Equals("stdlib") );
            Assert.NotNull(name);

            var models = client.ListModels(Dbname, EngineName);
            var model = models.Find( m => m.Name.Equals("stdlib") );
            Assert.NotNull(model);

            model = client.GetModel(Dbname, EngineName, "stdlib");
            Assert.NotNull(model);
            Assert.True(model.Value.Length > 0);

            var deleteRsp = client.DeleteDatabase(Dbname);
            Assert.Equal(Dbname, deleteRsp.Name);

            Assert.Throws<SystemException>( () => client.GetDatabase(Dbname) );
        }

        string testModel =
                "def R = \"hello\", \"world\"";

        string testJson = "{" +
                "\"name\":\"Amira\",\n" +
                "\"age\":32,\n" +
                "\"height\":null,\n" +
                "\"pets\":[\"dog\",\"rabbit\"]}";

        [Fact]
        public void DatabaseCloneTest()
        {
            Client client = CreateClient();
            client.CreateEngineWait(EngineName, size: EngineSize.XS);

            Assert.Throws<SystemException>( () => client.DeleteDatabase(Dbname) );

            // create a fresh database
            var createRsp = client.CreateDatabase(Dbname, EngineName);
            Assert.Equal(Dbname, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            // load some data and model
            var loadRsp = client.LoadJson(Dbname, EngineName, "test_data", testJson);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            loadRsp = client.LoadModel(Dbname, EngineName, "test_model", testModel);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            // clone database
            var databaseCloneName = $"{Dbname}-clone";
            createRsp = client.CloneDatabase(databaseCloneName, EngineName, Dbname, true);
            Assert.Equal(databaseCloneName, createRsp.Name);
            Assert.Equal("CREATED", createRsp.State);

            // make sure the database exists
            var database = client.GetDatabase(databaseCloneName);
            Assert.Equal(databaseCloneName, database.Name);
            Assert.Equal("CREATED", database.State);

            var databases = client.ListDatabases();
            database = databases.Find( db => db.Name == databaseCloneName );
            Assert.NotNull(database);
            Assert.Equal(databaseCloneName, database.Name);
            Assert.Equal("CREATED", database.State);

            // make sure the data was cloned
            var rsp = client.Execute(databaseCloneName, EngineName, "test_data", true);

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
            var modelNames = client.ListModelNames(databaseCloneName, EngineName);
            var name = modelNames.Find( item => item.Equals("test_model") );
            Assert.NotNull(name);

            var models = client.ListModels(databaseCloneName, EngineName);
            var model = models.Find( m => m.Name.Equals("test_model") );
            Assert.NotNull(model);

            model = client.GetModel(databaseCloneName, EngineName, "test_model");
            Assert.NotNull(model);
            Assert.True(model.Value.Length > 0);

            // cleanup
            var deleteRsp = client.DeleteDatabase(databaseCloneName);
            Assert.Equal(databaseCloneName, deleteRsp.Name);
        }
    }
}
