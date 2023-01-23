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
            WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            testOutputHelper.WriteLine(message);
        }
    }
}