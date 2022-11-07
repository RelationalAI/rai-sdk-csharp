using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class EngineTests : UnitTest
    {

        [Fact]
        public async Task EngineTest()
        {
            var client = CreateClient();

            var createRsp = await CreateEngineWaitAsync(client);
            Assert.Equal(createRsp.Name, GetEngineName());
            Assert.Equal(EngineStates.Provisioned, createRsp.State);

            var engine = await client.GetEngineAsync(createRsp.Name);
            Assert.Equal(engine.Name, createRsp.Name);
            Assert.Equal(EngineStates.Provisioned, engine.State);

            var engines = await client.ListEnginesAsync();
            engine = engines.Find(item => item.Name.Equals(createRsp.Name));
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync(EngineStates.Provisioned);
            engine = engines.Find(item => item.Name.Equals(createRsp.Name));
            Assert.NotNull(engine);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.DeleteEngineWaitAsync(createRsp.Name));

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.GetEngineAsync(createRsp.Name));
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

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
