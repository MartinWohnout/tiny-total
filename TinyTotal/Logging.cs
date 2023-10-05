using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace TinyTotal
{
    internal class Logging
    {
        static Logging()
        {
            Instance = new Logging();
        }

        private Logging()
        {
            MainLogger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(GetLogFilePath(), rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }

        public static Logging Instance { get; private set; }

        public void LogDebug(string message) => Log(message, LogEventLevel.Debug);
        public void LogInfo(string message) => Log(message, LogEventLevel.Information);
        public void LogWarn(string message) => Log(message, LogEventLevel.Warning);
        public void Log(Exception ex, string message = null)
        {
            MainLogger.Write(LogEventLevel.Error, ex, message ?? string.Empty);
        }
        internal void Log(string message, LogEventLevel logLevel)
        {
            MainLogger.Write(logLevel, message);
        }


        private Logger MainLogger { get; init; }

        private string GetLogFilePath()
        {
            string appName = System.AppDomain.CurrentDomain.FriendlyName;
            var sanitizedAppName = string.Join('_', appName.Split(Path.GetInvalidFileNameChars()));
            string fileName = "Logfile.txt";
            return Path.Join(Path.GetTempPath(), sanitizedAppName, fileName);
        }
    }
}
