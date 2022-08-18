using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class EngineTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string EngineName = $"csharp-sdk-{UUID}";

        [Fact]
        public async Task EngineTest()
        {
            var client = CreateClient();

            var createRsp = await client.CreateEngineWaitAsync(EngineName);
            Assert.Equal(createRsp.Name, EngineName);
            Assert.Equal(createRsp.State, "PROVISIONED");

            var engine = await client.GetEngineAsync(EngineName);
            Assert.Equal(engine.Name, EngineName);
            Assert.Equal(engine.State, "PROVISIONED");

            var engines = await client.ListEnginesAsync();
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync("PROVISIONED");
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.NotNull(engine);

            engines = await client.ListEnginesAsync("NONSENSE");
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.Null(engine);

            await Assert.ThrowsAsync<SystemException>(async () => await client.DeleteEngineWaitAsync(EngineName) );

            await Assert.ThrowsAsync<SystemException>(async () => await client.GetEngineAsync(EngineName) );

            engines = await client.ListEnginesAsync();
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.Equal(engine.State, "DELETED");
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try { await client.DeleteEngineWaitAsync(EngineName); } catch {}
        }
    }
}
