using System;
using Xunit;

namespace RelationalAI.Test
{
    public class ModelsTests : UnitTest
    {
        string testModel = "def R = \"hello\", \"world\"";

        [Fact]
        public void ModelsTest()
        {
            Client client = CreateClient();

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var loadRsp = client.LoadModel(Dbname, EngineName, "test_model", testModel);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var model = client.GetModel(Dbname, EngineName, "test_model");
            Assert.Equal("test_model", model.Name);

            var modelNames = client.ListModelNames(Dbname, EngineName);
            var modelName = modelNames.Find( item => item.Equals("test_model") );

            var models = client.ListModels(Dbname, EngineName);
            model = models.Find( item => item.Name.Equals("test_model") );
            Assert.NotNull(model);

            var deleteRsp = client.DeleteModel(Dbname, EngineName, "test_model");
            Assert.Equal(false, deleteRsp.Aborted);
            Assert.Equal(0, deleteRsp.Output.Length);
            Assert.Equal(0, deleteRsp.Problems.Length);

            Assert.Throws<SystemException>( () => client.GetModel(Dbname, EngineName, "test_model") );

            modelNames = client.ListModelNames(Dbname, EngineName);
            modelName = modelNames.Find( item => item.Equals("test_model") );
            Assert.Null(modelName);

            models = client.ListModels(Dbname, EngineName);
            model = models.Find( item => item.Name.Equals("test_model") );
            Assert.Null(model);
        }
    }
}
