namespace RelationalAI
{
    // an interface to for loggers implementations
    public interface IRAILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Fatal(string message);
        void Trace(string message);
    }
}
