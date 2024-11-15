using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using A9N.FlexTimeMonitor.DataAccess;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Views;
using A9N.FlexTimeMonitor.Win32;
using NLog;

namespace A9N.FlexTimeMonitor
{
    public partial class MainView : System.Windows.Window
    {
        // Keep reference to prevent GC to collect item
        // ReSharper disable once NotAccessedField.Local
        private readonly NotificationIcon _notificationIcon;
        // ReSharper disable once NotAccessedField.Local
        private readonly PowerModeObserver _powerModeObserver;
        private readonly ILogger _log = LogManager.GetLogger(nameof(MainView));
        
        public MainView()
        {
            InitializeComponent();

            UpdateSettings();

            var historyService = new WorkHistoryService(Properties.Resources.ApplicationName);
            DataContext = new MainViewModel(new MenuViewModel(this), historyService);

            Load();

            _powerModeObserver = new PowerModeObserver(Save);

            _notificationIcon = new NotificationIcon(
                Properties.Resources.ApplicationName,
                Properties.Resources.ApplicationIcon,
                () => WindowState = WindowState.Normal,
                () => (DataContext as MainViewModel)?.BalloonText);
        }

        private static void UpdateSettings()
        {
            if (Settings.Default.UpdateRequired)
            {
                Settings.Default.Upgrade();
                // Custom user variable that triggers the update process (true by default)
                Settings.Default.UpdateRequired = false;
                Settings.Default.Save();
            }
        }

        private void Load()
        {
            try
            {
                (DataContext as MainViewModel)?.OpenHistory();

                WorkdayGrid.SortByDateDescending();
            }
            catch (Exception e)
            {
                _log.Error(e);

                var text = String.Format(Properties.Resources.Status_ErrorLoadingHistory, e);

                MessageBox.Show(this, text, Properties.Resources.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void Save()
        {
            try
            {
                WorkdayGrid.CommitChanges();

                (DataContext as MainViewModel)?.SaveHistory();
            }
            catch (Exception e)
            {
                _log.Error(e);

                MessageBox.Show(this, e.Message, Properties.Resources.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            ShowInTaskbar = WindowState != WindowState.Minimized;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Save();
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Save();
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}