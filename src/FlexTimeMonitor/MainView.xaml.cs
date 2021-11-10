using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using A9N.FlexTimeMonitor.DataAccess;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Views;
using Microsoft.Win32;
using NLog;

namespace A9N.FlexTimeMonitor
{
    public partial class MainView : Window
    {
        // Keep reference to prevent GC to collect item
        private readonly NotificationIcon notificationIcon;
        private readonly ILogger log = LogManager.GetLogger(nameof(MainView));

        public MainView()
        {
            InitializeComponent();

            UpdateSettings();

            var historyService = new WorkHistoryService(Properties.Resources.ApplicationName);
            DataContext = new MainViewModel(new MenuViewModel(this), historyService);

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            Load();

            notificationIcon = new NotificationIcon(
                Properties.Resources.ApplicationName,
                Properties.Resources.ApplicationIcon,
                state => WindowState = state,
                () => (DataContext as MainViewModel)?.BalloonText);
        }

        private static void UpdateSettings()
        {
            if (Settings.Default.UpdateSettings)
            {
                Settings.Default.Upgrade();
                // Custom user variable that triggers the update process (true by default)
                Settings.Default.UpdateSettings = false;
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
                log.Error(e);

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
                log.Error(e);

                MessageBox.Show(this, e.Message, Properties.Resources.ApplicationName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                Save();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            ShowInTaskbar = WindowState == WindowState.Minimized ? false : true;
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