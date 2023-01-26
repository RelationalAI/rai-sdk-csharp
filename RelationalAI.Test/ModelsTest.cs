using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class ModelsTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        private readonly Dictionary<string, string> TestModel = new Dictionary<string, string> { { "test_model", "def R = \"hello\", \"world\"" } };

        private readonly ITestOutputHelper outputHelper;
        private readonly EngineFixture engineFixture;

        public ModelsTests(EngineFixture fixture, ITestOutputHelper output)
        {
            outputHelper = output;
            engineFixture = fixture;
        }

        [Fact]
        public async Task ModelsTest()
        {
            var client = CreateClient(outputHelper);

            await engineFixture.CreateEngineWaitAsync();
            await client.CreateDatabaseAsync(Dbname, engineFixture.Engine.Name);

            var resp = await client.LoadModelsWaitAsync(Dbname, engineFixture.Engine.Name, TestModel);
            Assert.Equal(TransactionAsyncState.Completed, resp.Transaction.State);
            Assert.Empty(resp.Problems);

            var model = await client.GetModelAsync(Dbname, engineFixture.Engine.Name, "test_model");
            Assert.Equal("test_model", model.Name);
            Assert.Equal(TestModel["test_model"], model.Value);

            var modelNames = await client.ListModelsAsync(Dbname, engineFixture.Engine.Name);
            var modelName = modelNames.Find(item => item.Equals("test_model"));

            var deleteRsp = await client.DeleteModelsAsync(Dbname, engineFixture.Engine.Name, new List<string> { "test_model" });
            Assert.Equal(TransactionAsyncState.Completed, deleteRsp.Transaction.State);
            Assert.Empty(deleteRsp.Problems);

            await Assert.ThrowsAsync<HttpError>(async () => await client.GetModelAsync(Dbname, engineFixture.Engine.Name, "test_model"));

            modelNames = await client.ListModelsAsync(Dbname, engineFixture.Engine.Name);
            modelName = modelNames.Find(item => item.Equals("test_model"));
            Assert.Null(modelName);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient(outputHelper);

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
