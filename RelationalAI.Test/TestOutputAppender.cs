using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class TestOutputAppender : AppenderSkeleton
    {
        private readonly ITestOutputHelper _outputHelper;
        public TestOutputAppender(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
            Layout = new PatternLayout("%date [%thread] %-5level %logger - %message%newline");
        }

        protected override void Append(LoggingEvent loggingEvent)
        {
            _outputHelper.WriteLine(RenderLoggingEvent(loggingEvent));
        }
    }
}
