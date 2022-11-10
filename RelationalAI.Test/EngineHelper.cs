using System;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using RelationalAI;

namespace RelationalAI.Test
{
    public class EngineHelper
    {
        
        private static Engine _engine;
        private static int _countEngines;
        private static readonly SemaphoreSlim _semaphoreEngine = new SemaphoreSlim(1);
        private static readonly EngineHelper _instance = new EngineHelper();
        private string engineName = "sdk-csharp-engine-" + Guid.NewGuid().ToString();

        private EngineHelper() {}
        
        public static EngineHelper Instance    
        {    
            get    
            {    
                return _instance;    
            }    
        }

        public string EngineName => engineName; 

        public async Task<Engine> CreateOrGetEngineAsync() 
        {  
            try
            {
                await _semaphoreEngine.WaitAsync();
                if (_engine == null)
                {
                    var ut = new UnitTest();
                    var client = ut.CreateClient();
                    try
                    {
                        _engine = await client.GetEngineAsync(EngineName);
                    }
                    catch(NotFoundException)
                    {
                        _engine = await client.CreateEngineWaitAsync(EngineName);
                        Assert.Equal(_engine.Name, EngineName);
                        Assert.Equal(EngineStates.Provisioned, _engine.State);
                    }
                }
                _countEngines++;
                return _engine;
            }
            finally 
            {
               _semaphoreEngine.Release();     
            }
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public async void DeleteEngineAsync()
        {
            try
            {
                await _semaphoreEngine.WaitAsync();
                _countEngines--;
                if (_countEngines <= 0)
                {
                    var ut = new UnitTest();
                    var client = ut.CreateClient();
                    await client.DeleteEngineWaitAsync(EngineName);
                    _engine = null;
                }
            }
            catch(NotFoundException)
            {
               // No-op    
            }
            finally
            {
                _semaphoreEngine.Release();
            }
        }
     }
}