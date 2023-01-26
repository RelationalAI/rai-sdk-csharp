using System;
using System.Collections;
using System.Collections.Generic;

namespace RelationalAI
{
    public static class LoggerFactory
    {
        private static IDictionary _loggers = new Dictionary<string, IRAILogger>();

        public static RAITraceSourceLogger GetRAITraceSourceLogger(string name)
        {
            if (name == null)
            {
                throw new ArgumentException($"logger name should not be null");
            }

            if (!_loggers.Contains(name))
            {
                _loggers.Add(name, new RAITraceSourceLogger(name));
            }

            return _loggers[name] as RAITraceSourceLogger;
        }
    }
}
