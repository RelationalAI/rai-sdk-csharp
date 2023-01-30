using System;
using Microsoft.Extensions.Logging;

namespace RelationalAI
{
    // RAILoggerManager is used to inject
    // ILoggerFactory
    public static class RAILoggerManager
    {
        public static ILoggerFactory LoggerFactory = new LoggerFactory();
    }
}
