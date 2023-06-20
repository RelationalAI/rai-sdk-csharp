using System;
using System.Threading.Tasks;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Logging;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository;

namespace RelationalAI.Test
{
    public class EngineFixture : IDisposable
    {
        private readonly string engineName = "sdk-csharp-" + Environment.GetEnvironmentVariable("RunId");
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

        public async Task<Engine> CreateEngineWaitAsync(Client client)
        {
            try
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    Engine = await client.GetEngineAsync(engineName);
                    if (Engine.State != "PROVISIONED")
                    {
                        throw new EngineProvisionFailedException(Engine);
                    }

                    return Engine;
                }
                catch (HttpError ex)
                {
                    if (ex.StatusCode != 404)
                    {
                        throw ex;
                    }
                }

                if (Engine == null)
                {
                    Engine = await client.CreateEngineWaitAsync(engineName);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }


            return Engine;
        }

        public Engine Engine { get; private set; }
    }

    [CollectionDefinition("RelationalAI.Test")]
    public class RelationalAITestCollection : ICollectionFixture<EngineFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }


    // log4net custom appender
    internal class RAITestOutputAppender : AppenderSkeleton
    {
        private readonly ITestOutputHelper _outputHelper;

        public RAITestOutputAppender(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            Name = GetTestContext(outputHelper).TestCase.TestMethod.Method.Name;
            Layout = new PatternLayout("%date [%property{appender}] [%thread] %-5level %logger - %message");
        }

        protected override void Append(log4net.Core.LoggingEvent loggingEvent)
        {
            if (log4net.LogicalThreadContext.Properties["appender"].Equals(Name))
            {
                _outputHelper.WriteLine(RenderLoggingEvent(loggingEvent));
            }
        }

        protected ITest GetTestContext(ITestOutputHelper outputHelper)
        {
            return (ITest)outputHelper.GetType()
                .GetField("test", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(outputHelper);
        }
    }

    // log4net custom provider
    internal class RAILog4NetProvider : ILoggerProvider
    {
        private readonly ILoggerProvider _defaultLog4NetProvider;
        private readonly ILoggerRepository _loggerRepository;
        private log4net.Core.IAppenderAttachable _attachable;


        public RAILog4NetProvider()
        {
            _defaultLog4NetProvider = new Log4NetProvider();
            _loggerRepository = log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());
            _attachable = ((log4net.Repository.Hierarchy.Hierarchy)_loggerRepository).Root;
        }

        public void AddRAITestOutputAppender(ITestOutputHelper outputHelper)
        {
            var appender = new RAITestOutputAppender(outputHelper);
            log4net.LogicalThreadContext.Properties["appender"] = appender.Name;
            _attachable.AddAppender(appender);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _defaultLog4NetProvider.CreateLogger(categoryName);
        }

        public void Dispose()
        {
            // cleanup appenders
            _defaultLog4NetProvider.Dispose();
        }
    }
}
