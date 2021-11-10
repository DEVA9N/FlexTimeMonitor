using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.Facility
{
    internal class AppLogging
    {
        internal static void Inititialize()
        {
            LogManager.Configuration = CreateLoggerConfig();
        }

        private static NLog.Config.LoggingConfiguration CreateLoggerConfig()
        {
            var logFileName = CreateLogFileName();
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = logFileName, Layout = "${longdate}|${level:uppercase=true}|${logger}|${callsite}|${message}" };

            var config = new NLog.Config.LoggingConfiguration();
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);

            return config;
        }

        private static string CreateLogFileName()
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var folder = "Flex Time Monitor";
            var file = "FlexTimeMonitor.log";
            var logFileName = Path.Combine(appData, folder, file);

            return logFileName;
        }
    }
}
