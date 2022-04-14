using System;
using System.Collections.Generic;
using Xunit;

namespace RelationalAI.Test
{
    public class EngineTests : UnitTest
    {

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

            client.DeleteEngineWait(EngineName);

            var rsp = client.GetEngine(EngineName);
            Assert.Equal(rsp.Name, EngineName);
            Assert.Equal("DELETED", rsp.State);

            engines = client.ListEngines();
            engine = engines.Find( item => item.Name.Equals(EngineName) );
            Assert.Equal(engine.State, "DELETED");
        }
    }
}
