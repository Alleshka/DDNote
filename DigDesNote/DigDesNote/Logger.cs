using NLog;

namespace DigDesNote.Logger
{
    public static class Log
    {
        public static readonly NLog.Logger Instance = LogManager.GetCurrentClassLogger();
    }
}
