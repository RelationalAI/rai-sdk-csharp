using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class ModelsTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{UUID}";
        public static string EngineName = $"csharp-sdk-{UUID}";
        string testModel = "def R = \"hello\", \"world\"";

        [Fact]
        public async Task ModelsTest()
        {
            Client client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var loadRsp = await client.LoadModelAsync(Dbname, EngineName, "test_model", testModel);
            Assert.Equal(false, loadRsp.Aborted);
            Assert.Equal(0, loadRsp.Output.Length);
            Assert.Equal(0, loadRsp.Problems.Length);

            var model = await client.GetModelAsync(Dbname, EngineName, "test_model");
            Assert.Equal("test_model", model.Name);

            var modelNames = await client.ListModelNamesAsync(Dbname, EngineName);
            var modelName = modelNames.Find( item => item.Equals("test_model") );

            var models = await client.ListModelsAsync(Dbname, EngineName);
            model = models.Find( item => item.Name.Equals("test_model") );
            Assert.NotNull(model);

            var deleteRsp = await client.DeleteModelAsync(Dbname, EngineName, "test_model");
            Assert.Equal(false, deleteRsp.Aborted);
            Assert.Equal(0, deleteRsp.Output.Length);
            Assert.Equal(0, deleteRsp.Problems.Length);

            await Assert.ThrowsAsync<SystemException>(async () => await client.GetModelAsync(Dbname, EngineName, "test_model") );

            modelNames = await client.ListModelNamesAsync(Dbname, EngineName);
            modelName = modelNames.Find( item => item.Equals("test_model") );
            Assert.Null(modelName);

            models = await client.ListModelsAsync(Dbname, EngineName);
            model = models.Find( item => item.Name.Equals("test_model") );
            Assert.Null(model);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteDatabaseAsync(Dbname); } catch {}
            try { await client.DeleteEngineWaitAsync(EngineName); } catch {}
        }
    }
}
