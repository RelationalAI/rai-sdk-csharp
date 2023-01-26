using System.Diagnostics;

namespace RelationalAI
{
    public class RAITraceSourceLogger : IRAILogger
    {
        private readonly int _loggerId;
        private readonly TraceSource _traceSource;

        public RAITraceSourceLogger(string name, int id = 100)
        {
            _traceSource = new TraceSource(name, SourceLevels.All);
            _loggerId = id;
        }

        public void Debug(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, _loggerId, message);
            _traceSource.Flush();
        }

        public void Error(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Error, _loggerId, message);
            _traceSource.Flush();
        }

        public void Fatal(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Critical, _loggerId, message);
            _traceSource.Flush();
        }

        public void Info(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Information, _loggerId, message);
            _traceSource.Flush();
        }

        public void Trace(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Verbose, _loggerId, message);
            _traceSource.Flush();
        }

        public void Warning(string message)
        {
            _traceSource.TraceEvent(TraceEventType.Warning, _loggerId, message);
            _traceSource.Flush();
        }

        public int AddListener(TraceListener listener)
        {
            return _traceSource.Listeners.Add(listener);
        }

        public bool SwitchLogLevel(TraceEventType eventType)
        {
            return _traceSource.Switch.ShouldTrace(eventType);
        }
    }
}
