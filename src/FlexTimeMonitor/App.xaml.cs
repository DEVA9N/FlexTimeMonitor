using System;
using System.Reflection;
using System.Windows;
using System.Threading.Tasks;
using NLog;
using A9N.FlexTimeMonitor.Infrastructure;
using A9N.FlexTimeMonitor.Properties;
using Microsoft.VisualBasic.Logging;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ILogger _logger;

        public App()
        {
            if (!SingleInstance.CheckIsFirstInstance())
            {
                Current.Shutdown();
            }

            AppLogging.Initialize();
            _logger = LogManager.GetLogger(GetType().Name);

            LogApplicationStart();

            // See https://stackoverflow.com/questions/1472498/wpf-global-exception-handler/1472562#1472562
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void LogApplicationStart()
        {
            var appInfo = $"FlexTimeMonitor {Assembly.GetExecutingAssembly().GetName().Version} started";

            _logger.Info(new String('-', appInfo.Length));
            _logger.Info(appInfo);
            _logger.Info(new String('-', appInfo.Length));
        }

        private void Dispatcher_UnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                HandleException(e.Exception);
            }
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleException(exception);
            }
        }

        private void HandleException(Exception e)
        {
            _logger.Fatal(e, "Unhandled Exception occured.");
        }
    }
}
