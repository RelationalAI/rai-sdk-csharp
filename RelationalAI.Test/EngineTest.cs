using System;
using System.Threading.Tasks;
using RelationalAI.Models.Engine;
using Xunit;

namespace RelationalAI.Test
{
    public class EngineTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string EngineName = $"csharp-sdk-{Uuid}";

        [Fact]
        public async Task EngineTest()
        {
            var client = CreateClient();

            var createRsp = await client.CreateEngineWaitAsync(EngineName);
            Assert.Equal(createRsp.Name, EngineName);
            Assert.True(EngineState.Provisioned.IsEqual(createRsp.State));

            var engine = await client.GetEngineAsync(EngineName);
            Assert.Equal(engine.Name, EngineName);
            Assert.True(EngineState.Provisioned.IsEqual(engine.State));

            var engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync(EngineState.Provisioned.Value());
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync("NONSENSE");
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.Null(engine);

            await Assert.ThrowsAsync<SystemException>(async () => await client.DeleteEngineWaitAsync(EngineName));

            await Assert.ThrowsAsync<SystemException>(async () => await client.GetEngineAsync(EngineName));

            engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.True(EngineState.Deleted.IsEqual(engine.State));
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteEngineWaitAsync(EngineName); } catch { }
        }
    }
}
