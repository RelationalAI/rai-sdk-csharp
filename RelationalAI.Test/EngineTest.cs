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
            var engine = await CreateEngineWaitAsync(EngineName);
            Assert.Equal(engine.Name, GetEngineName(EngineName));
            Assert.Equal(EngineStates.Provisioned, engine.State);

            engine = await client.GetEngineAsync(engine.Name);
            Assert.Equal(engine.Name, GetEngineName(EngineName));
            Assert.Equal(EngineStates.Provisioned, engine.State);

            //var engines = await client.ListEnginesAsync();
            //engine = engines.Find(item => item.Name.Equals(EngineName));
            //Assert.NotNull(engine);

            var engines = await client.ListEnginesAsync(EngineStates.Provisioned);
            engine = engines.Find(item => item.Name.Equals(GetEngineName(EngineName)));
            Assert.NotNull(engine);
        }

        public override async Task DisposeAsync()
        {
            try
            {
                DeleteEngineWaitAsync(EngineName);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
