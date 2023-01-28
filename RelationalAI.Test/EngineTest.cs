using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class EngineTests : UnitTest
    {
        private readonly EngineFixture engineFixture;

        public EngineTests(EngineFixture fixture, ITestOutputHelper output) : base(output)
        {
            engineFixture = fixture;
        }

        [Fact]
        public async Task EngineTest()
        {
            var client = CreateClient();

            await engineFixture.CreateEngineWaitAsync();

            engineFixture.Engine.State.Should().Be(EngineStates.Provisioned);

            var engine = await client.GetEngineAsync(engineFixture.Engine.Name);
            engine.Name.Should().Be(engineFixture.Engine.Name);
            engine.State.Should().Be(EngineStates.Provisioned);

            var engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(engineFixture.Engine.Name));
            engine.Should().NotBeNull();

            engines = await client.ListEnginesAsync(EngineStates.Provisioned);
            engine = engines.Find(item => item.Name.Equals(engineFixture.Engine.Name));
            engine.Should().NotBeNull();
        }
    }
}
