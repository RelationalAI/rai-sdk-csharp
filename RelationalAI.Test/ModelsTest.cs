using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class ModelsTests : UnitTest
    {
        public static string Dbname = $"csharp-sdk-{Uuid}";

        private readonly Dictionary<string, string> TestModel = new Dictionary<string, string> { { "test_model", "def R = \"hello\", \"world\"" } };

        [Fact]
        public async Task ModelsTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            await client.CreateDatabaseAsync(Dbname, createRsp.Name);

            var resp = await client.LoadModelsWaitAsync(Dbname, createRsp.Name, TestModel);
            Assert.Equal(TransactionAsyncState.Completed, resp.Transaction.State);
            Assert.Empty(resp.Problems);

            var model = await client.GetModelAsync(Dbname, createRsp.Name, "test_model");
            Assert.Equal("test_model", model.Name);
            Assert.Equal(TestModel["test_model"], model.Value);

            var modelNames = await client.ListModelsAsync(Dbname, createRsp.Name);
            var modelName = modelNames.Find(item => item.Equals("test_model"));

            var deleteRsp = await client.DeleteModelsAsync(Dbname, createRsp.Name, new List<string> { "test_model" });
            Assert.Equal(TransactionAsyncState.Completed, deleteRsp.Transaction.State);
            Assert.Empty(deleteRsp.Problems);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.GetModelAsync(Dbname, createRsp.Name, "test_model"));

            modelNames = await client.ListModelsAsync(Dbname, createRsp.Name);
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
                await client.DeleteEngineWaitAsync(GetEngineName());
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
