using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class ModelsTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        public static string EngineName = $"csharp-sdk-{Uuid}";
        string _testModel = "def R = \"hello\", \"world\"";

        [Fact]
        public async Task ModelsTest()
        {
            var client = CreateClient();

            await client.CreateEngineWaitAsync(EngineName);
            await client.CreateDatabaseAsync(Dbname, EngineName);

            var loadRsp = await client.LoadModelAsync(Dbname, EngineName, "test_model", _testModel);
            Assert.False(loadRsp.Aborted);
            Assert.Empty(loadRsp.Output);
            Assert.Empty(loadRsp.Problems);

            var model = await client.GetModelAsync(Dbname, EngineName, "test_model");
            Assert.Equal("test_model", model.Name);

            var modelNames = await client.ListModelNamesAsync(Dbname, EngineName);
            var modelName = modelNames.Find(item => item.Equals("test_model"));

            var models = await client.ListModelsAsync(Dbname, EngineName);
            model = models.Find(item => item.Name.Equals("test_model"));
            Assert.NotNull(model);

            var deleteRsp = await client.DeleteModelAsync(Dbname, EngineName, "test_model");
            Assert.False(deleteRsp.Aborted);
            Assert.Empty(deleteRsp.Output);
            Assert.Empty(deleteRsp.Problems);

            await Assert.ThrowsAsync<SystemException>(async () => await client.GetModelAsync(Dbname, EngineName, "test_model"));

            modelNames = await client.ListModelNamesAsync(Dbname, EngineName);
            modelName = modelNames.Find(item => item.Equals("test_model"));
            Assert.Null(modelName);

            models = await client.ListModelsAsync(Dbname, EngineName);
            model = models.Find(item => item.Name.Equals("test_model"));
            Assert.Null(model);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteDatabaseAsync(Dbname); } catch { }
            try { await client.DeleteEngineWaitAsync(EngineName); } catch { }
        }
    }
}
