using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class EngineTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        private Engine _engine;

        [Fact]
        public async Task EngineTest()
        {
            var client = CreateClient();

            var engineName = EngineHelper.Instance.EngineName;
            Assert.Equal(_engine.Name, engineName);
            Assert.Equal(EngineStates.Provisioned, _engine.State);

            var engine = await client.GetEngineAsync(engineName);
            Assert.Equal(engine.Name, engineName);
            Assert.Equal(EngineStates.Provisioned, engine.State);

            //var engines = await client.ListEnginesAsync();
            //engine = engines.Find(item => item.Name.Equals(engineName));
            //Assert.NotNull(engine);

            var engines = await client.ListEnginesAsync(EngineStates.Provisioned);
            engine = engines.Find(item => item.Name.Equals(engineName));
            Assert.NotNull(engine);
        }

        public override async Task DisposeAsync()
        {
            try
            {
                EngineHelper.Instance.DeleteEngineAsync();
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
        public override async Task InitializeAsync()
        {
             _engine = await EngineHelper.Instance.CreateOrGetEngineAsync();
        }
    }
}
