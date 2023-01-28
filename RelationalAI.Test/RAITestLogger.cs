using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class RAITestLogger : IDisposable
    {
        private readonly IAppenderAttachable _attachable;
        private readonly TestOutputAppender _appender;

        public RAITestLogger(ITestOutputHelper outputHelper)
        {
            ILoggerRepository repo = LogManager.GetRepository(Assembly.GetExecutingAssembly());
            XmlConfigurator.Configure(repo);
            _attachable = ((log4net.Repository.Hierarchy.Hierarchy)repo).Root;

            _appender = new TestOutputAppender(outputHelper);
            if (_appender != null)
            {
                _attachable.AddAppender(_appender);
            }
        }

        /*public RAITestLogger(ITestOutputHelper outputHelper)
        {
            var repositoryName = $"{Thread.CurrentThread.ManagedThreadId}";
            var repo = GetOrCreateRepository(repositoryName);
            _appender = new TestOutputAppender(outputHelper);
            BasicConfigurator.Configure(repo, _appender);
        }

        private ILoggerRepository GetOrCreateRepository(string name)
        {
            var allRepos = LogManager.GetAllRepositories();
            ILoggerRepository repo = allRepos.FirstOrDefault(x => x.Name == name);
            if (repo == null)
            {
                return LoggerManager.CreateRepository(name);
            }

            return repo;
        }*/


        public void Dispose()
        {
            _attachable.RemoveAppender(_appender);
        }
    }
}
