using System.Diagnostics;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class RAITestTraceListener : TraceListener
    {
        private readonly ITestOutputHelper testOutputHelper;

        public RAITestTraceListener(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }
        public override void Write(string message)
        {
            // ITestOutputHelper has no Write function
            // using WriteLine for the moment until we figure
            // out a better way to this
            testOutputHelper.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            testOutputHelper.WriteLine(message);
        }
    }
}