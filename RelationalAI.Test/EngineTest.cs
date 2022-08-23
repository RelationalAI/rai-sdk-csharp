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
            Assert.Equal(EngineState.Provisioned, createRsp.State);

            var engine = await client.GetEngineAsync(EngineName);
            Assert.Equal(engine.Name, EngineName);
            Assert.Equal(EngineState.Provisioned, engine.State);

            var engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync(EngineState.Provisioned);
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.NotNull(engine);

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                client.ListEnginesAsync((EngineState)1500));

            await Assert.ThrowsAsync<SystemException>(async () => await client.DeleteEngineWaitAsync(EngineName));

            await Assert.ThrowsAsync<SystemException>(async () => await client.GetEngineAsync(EngineName));

            engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.Equal(EngineState.Deleted, engine.State);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteEngineWaitAsync(EngineName); } catch { }
        }
    }
}
