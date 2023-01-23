using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class EngineTests : UnitTest
    {
        private readonly ITestOutputHelper outputHelper;
        private readonly EngineFixture engineFixture;

        public EngineTests(EngineFixture fixture, ITestOutputHelper output)
        {
            outputHelper = output;
            engineFixture = fixture;
        }

        [Fact]
        public async Task EngineTest()
        {
            var client = CreateClient(outputHelper);

            await engineFixture.CreateEngineWaitAsync();

            Assert.Equal(EngineStates.Provisioned, engineFixture.Engine.State);

            var engine = await client.GetEngineAsync(engineFixture.Engine.Name);
            Assert.Equal(engine.Name, engineFixture.Engine.Name);
            Assert.Equal(EngineStates.Provisioned, engine.State);

            var engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(engineFixture.Engine.Name));
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync(EngineStates.Provisioned);
            engine = engines.Find(item => item.Name.Equals(engineFixture.Engine.Name));
            Assert.NotNull(engine);
        }
    }
}
