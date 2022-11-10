using System;
using System.Threading.Tasks;
using Xunit;
using RelationalAI;

namespace RelationalAI.Test
{
    public class EngineFixture : IDisposable
    {
        private Engine _engine;
        private string engineName = "sdk-csharp-" + Guid.NewGuid().ToString();

        public EngineFixture()
        {
            Task.Run(() => CreateEngineWaitAsync()).Wait();
        }

        public void Dispose()
        {
            try
            {
                DeleteEngineWaitAsync();
            }
            catch (System.Exception e)
            {
                Console.Error.WriteLineAsync(e.ToString());
            }
        }

        private async void CreateEngineWaitAsync()
        {
            var ut = new UnitTest();
            var client = ut.CreateClient();
            try
            {
                _engine = await client.CreateEngineWaitAsync(engineName);    
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
            }
            
        }

        private async void DeleteEngineWaitAsync()
        {
            var ut = new UnitTest();
            var client = ut.CreateClient();
            await client.DeleteEngineWaitAsync(engineName);    
        }

        public Engine Engine
        {
            get
            {
                return _engine;
            }
        }
    }

    [CollectionDefinition("RelationalAI.Test")]
    public class RelationalAITestCollection : ICollectionFixture<EngineFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }

}