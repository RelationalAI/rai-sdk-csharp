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
        private static int _countDeleteEngines;
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
                Console.WriteLine("Get Engine Count = " + _countEngines);
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
                _countDeleteEngines++;
                Console.WriteLine("Delete Engine Count = " + _countDeleteEngines);
                if (_countDeleteEngines >= _countEngines)
                {
                    var ut = new UnitTest();
                    var client = ut.CreateClient();
                    await client.DeleteEngineWaitAsync(EngineName);
                    Console.WriteLine("Engine Deleted");
                    _engine = null;
                    _countEngines = 0;
                    _countDeleteEngines = 0;
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