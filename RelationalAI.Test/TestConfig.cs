using System;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using RelationalAI;
using log4net.Appender;
using Xunit.Abstractions;
using log4net.Layout;
using log4net.Core;
using log4net.Repository;
using log4net;
using log4net.Config;
using System.Reflection;

namespace RelationalAI.Test
{
    public class EngineFixture : IDisposable
    {
        private Engine _engine;
        private readonly string engineName = "sdk-csharp-" + Guid.NewGuid().ToString();
        // Semaphore is used to lock the CreateEngine function so that only one test creates the engine.
        private static readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);

        public void Dispose()
        {
            try
            {
                var ut = new UnitTest();
                var client = ut.CreateClient();
                client.DeleteEngineWaitAsync(engineName).Wait();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.ToString());
            }
        }

        public async Task<Engine> CreateEngineWaitAsync()
        {
            try
            {
                if (_engine == null)
                {
                    await semaphoreSlim.WaitAsync();
                    var ut = new UnitTest();
                    var client = ut.CreateClient();
                    _engine = await client.CreateEngineWaitAsync(engineName);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }


            return _engine;
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


    // log4net custom appender
    public class TestOutputAppender : AppenderSkeleton
    {
        private readonly ITestOutputHelper _outputHelper;
        public TestOutputAppender(string name, ITestOutputHelper outputHelper)
        {
            Name = name;
            _outputHelper = outputHelper;
            Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline");
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            if (log4net.LogicalThreadContext.Properties["appender"].Equals(Name))
            {
                _outputHelper.WriteLine(RenderLoggingEvent(loggingEvent));
            }
        }
    }

    public class RAITestLog4netConfiguration : IDisposable
    {
        private readonly IAppenderAttachable _attachable;
        private readonly TestOutputAppender _appender;

        public RAITestLog4netConfiguration(string name, ITestOutputHelper outputHelper)
        {
            ILoggerRepository repo = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(repo);
            _attachable = ((log4net.Repository.Hierarchy.Hierarchy)repo).Root;

            _appender = new TestOutputAppender(name, outputHelper);
            LogicalThreadContext.Properties["appender"] = name;
            if (_appender != null)
            {
                _attachable.AddAppender(_appender);
            }
        }

        public void Dispose()
        {
            _attachable.RemoveAppender(_appender);
        }
    }

}