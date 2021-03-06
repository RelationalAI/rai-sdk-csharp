using System;
using System.Collections.Generic;
using Xunit;

namespace RelationalAI.Test
{
    public class EngineTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string EngineName = $"csharp-sdk-{UUID}";

        [Fact]
        public void EnginetTest()
        {
            Client client = CreateClient();

            var createRsp = client.CreateEngineWait(EngineName);
            Assert.Equal(createRsp.Name, EngineName);
            Assert.Equal(createRsp.State, "PROVISIONED");

            var engine = client.GetEngine(EngineName);
            Assert.Equal(engine.Name, EngineName);
            Assert.Equal(engine.State, "PROVISIONED");

            var engines = client.ListEngines();
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.NotNull(engine);

            engines = client.ListEngines("PROVISIONED");
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.NotNull(engine);

            engines = client.ListEngines("NONSENSE");
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.Null(engine);

            Assert.Throws<SystemException>( () => client.DeleteEngineWait(EngineName) );

            Assert.Throws<SystemException>( () => client.GetEngine(EngineName) );

            engines = client.ListEngines();
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.Equal(engine.State, "DELETED");
        }

        public override void Dispose()
        {
            var client = CreateClient();

            try { client.DeleteEngineWait(EngineName); } catch {}
        }
    }
}
