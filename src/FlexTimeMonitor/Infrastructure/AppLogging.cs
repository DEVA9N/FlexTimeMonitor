using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.Infrastructure
{
    internal class AppLogging
    {
        internal static void Initialize()
        {
            LogManager.Configuration = CreateLoggerConfig();
        }

        private static LoggingConfiguration CreateLoggerConfig()
        {
            var logFileName = GetLogFileName();
            var logfile = new FileTarget("logfile") { FileName = logFileName, Layout = "${longdate}|${level:uppercase=true}|${logger}|${callsite}|${message}" };

            var config = new LoggingConfiguration();
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            return config;
        }

        private static string GetLogFileName()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = "Flex Time Monitor";
            var file = "FlexTimeMonitor.log";
            var logFileName = Path.Combine(appData, folder, file);

            return logFileName;
        }
    }
}
