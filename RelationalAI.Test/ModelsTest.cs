using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class ModelsTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";
        private readonly Dictionary<string, string> TestModel = new Dictionary<string, string> { { "test_model", "def R = \"hello\", \"world\"" } };

        private readonly EngineFixture engineFixture;

        public ModelsTests(EngineFixture fixture)
        {
            engineFixture = fixture;
        }

        [Fact]
        public async Task ModelsTest()
        {
            var client = CreateClient();

            await engineFixture.CreateEngineWaitAsync();
            engineFixture.Engine.State.Should().Be(EngineStates.Provisioned);

            await client.CreateDatabaseAsync(Dbname, engineFixture.Engine.Name);

            var resp = await client.LoadModelsWaitAsync(Dbname, engineFixture.Engine.Name, TestModel);
            resp.Transaction.State.Should().Be(TransactionAsyncState.Completed);
            resp.Problems.Should().HaveCount(0);

            var model = await client.GetModelAsync(Dbname, engineFixture.Engine.Name, "test_model");
            model.Name.Should().Be("test_model");
            model.Value.Should().Be(TestModel["test_model"]);

            var modelNames = await client.ListModelsAsync(Dbname, engineFixture.Engine.Name);
            var modelName = modelNames.Find(item => item.Equals("test_model"));

            var deleteRsp = await client.DeleteModelsAsync(Dbname, engineFixture.Engine.Name, new List<string> { "test_model" });
            deleteRsp.Transaction.State.Should().Be(TransactionAsyncState.Completed);
            deleteRsp.Problems.Should().HaveCount(0);

            await client
                .Invoking(c => c.GetModelAsync(Dbname, engineFixture.Engine.Name, "test_model"))
                .Should().ThrowAsync<HttpError>();

            modelNames = await client.ListModelsAsync(Dbname, engineFixture.Engine.Name);
            modelName = modelNames.Find(item => item.Equals("test_model"));
            modelName.Should().BeNull();
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
