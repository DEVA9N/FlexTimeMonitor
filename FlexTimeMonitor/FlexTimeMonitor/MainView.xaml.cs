using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using A9N.FlexTimeMonitor.DataAccess;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Views;
using Microsoft.Win32;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : Window
    {
        private readonly NotificationIcon notificationIcon;

        /// <summary>
        /// Gets or sets a value indicating whether to show the save dialog when the program is closing.
        /// </summary>
        /// <value><c>true</c> if the save dialog should be shown; otherwise, <c>false</c>.</value>
        internal bool ShowSaveDialog { get; set; } = true;

        public MainView()
        {
            InitializeComponent();

            var historyService = new WorkHistoryService(Properties.Resources.ApplicationName);
            DataContext = new MainViewModel(new MenuViewModel(this), historyService);

            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;

            UpdateSettings();

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
            // Will try to open an existing history file. If none is found it will create a new one.
            // If reading or writing fails the user is informed.
            try
            {
                (DataContext as MainViewModel)?.OpenHistory();

                WorkdayGrid.SortByDateDescending();
            }
            catch (Exception e)
            {
                var text = String.Format(Properties.Resources.Status_ErrorLoadingHistory, e);

                MessageBox.Show(this, text, Properties.Resources.ApplicationName);
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
                // When the user is saving the file via menu it is ok to display the error message
                if (ShowSaveDialog)
                {
                    // TODO: try to write log file, present log file on next application start
                    MessageBox.Show(this, e.Message, Properties.Resources.ApplicationName);
                }
            }
        }

        private void SaveSilent()
        {
            try
            {
                WorkdayGrid.CommitChanges();

                (DataContext as MainViewModel)?.SaveHistory();
            }
            catch (Exception)
            {
                // Don't notify about errors because there is no one listening when (e.g.) shutting down the system
            }
        }

        void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                SaveSilent();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            base.OnStateChanged(e);

            if (WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            if (WindowState != WindowState.Minimized)
            {
                this.ShowInTaskbar = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (ShowSaveDialog)
            {
                var result = MessageBox.Show(Properties.Resources.Message_Save_Text, Properties.Resources.Message_Save_Title, MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        Save();
                        e.Cancel = false;
                        break;
                    case MessageBoxResult.No:
                        e.Cancel = false;
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                    default:
                        e.Cancel = false;
                        break;
                }
            }
            else
            {
                SaveSilent();
            }
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