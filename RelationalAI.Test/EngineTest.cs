using System;
using System.Threading.Tasks;
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
            Assert.Equal(EngineStates.Provisioned, createRsp.State);

            var engine = await client.GetEngineAsync(EngineName);
            Assert.Equal(engine.Name, EngineName);
            Assert.Equal(EngineStates.Provisioned, engine.State);

            var engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync(EngineStates.Provisioned);
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.NotNull(engine);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.DeleteEngineWaitAsync(EngineName));

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.GetEngineAsync(EngineName));

            engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(EngineName));
            Assert.Equal(EngineStates.Deleted, engine.State);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try
            {
                await client.DeleteEngineWaitAsync(EngineName);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
