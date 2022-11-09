using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class ModelsTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";

        private readonly Dictionary<string, string> TestModel = new Dictionary<string, string> { { "test_model", "def R = \"hello\", \"world\"" } };

        [Fact]
        public async Task ModelsTest()
        {
            var client = CreateClient();

            var engineName = EngineHelper.Instance.EngineName; 
            await client.CreateDatabaseAsync(Dbname, engineName);

            var resp = await client.LoadModelsWaitAsync(Dbname, engineName, TestModel);
            Assert.Equal(TransactionAsyncState.Completed, resp.Transaction.State);
            Assert.Empty(resp.Problems);

            var model = await client.GetModelAsync(Dbname, engineName, "test_model");
            Assert.Equal("test_model", model.Name);
            Assert.Equal(TestModel["test_model"], model.Value);

            var modelNames = await client.ListModelsAsync(Dbname, engineName);
            var modelName = modelNames.Find(item => item.Equals("test_model"));

            var deleteRsp = await client.DeleteModelsAsync(Dbname, engineName, new List<string> { "test_model" });
            Assert.Equal(TransactionAsyncState.Completed, deleteRsp.Transaction.State);
            Assert.Empty(deleteRsp.Problems);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.GetModelAsync(Dbname, engineName, "test_model"));

            modelNames = await client.ListModelsAsync(Dbname, engineName);
            modelName = modelNames.Find(item => item.Equals("test_model"));
            Assert.Null(modelName);
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
                EngineHelper.Instance.DeleteEngineAsync();
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }

        public override async Task InitializeAsync()
        {
            await EngineHelper.Instance.CreateOrGetEngineAsync();
        }
    }
}
