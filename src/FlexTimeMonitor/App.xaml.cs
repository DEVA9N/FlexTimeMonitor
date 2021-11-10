using System;
using System.Windows;
using System.Threading.Tasks;
using NLog;
using A9N.FlexTimeMonitor.Facility;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            if (!SingleInstance.CheckIsFirstInstance())
            {
                Current.Shutdown();
            }

            AppLogging.Inititialize();

            // See https://stackoverflow.com/questions/1472498/wpf-global-exception-handler/1472562#1472562
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
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
            if (e.Exception != null)
            {
                HandleException(e.Exception);
            }
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
            var logger = LogManager.GetLogger(GetType().Name);

            logger.Fatal(e);
        }
    }
}
