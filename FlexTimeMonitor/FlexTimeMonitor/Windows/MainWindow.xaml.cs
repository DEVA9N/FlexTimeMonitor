using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.ViewModels;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon _notificationIcon;
        private const int BalloonTimeOut = 3000;

        /// <summary>
        /// Gets or sets a value indicating whether to show the save dialog when the program is closing.
        /// </summary>
        /// <value><c>true</c> if the save dialog should be shown; otherwise, <c>false</c>.</value>
        internal bool ShowSaveDialog { get; set; } = true;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel(this);

            InitializeNotificationIcon();

            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
      
            UpdateSettings();

            Load();
        }

        private void InitializeNotificationIcon()
        {
            this._notificationIcon = new System.Windows.Forms.NotifyIcon();
            this._notificationIcon.Icon = Properties.Resources.ApplicationIconLight;
            this._notificationIcon.Text = Properties.Resources.ApplicationName;
            this._notificationIcon.Visible = true;
            this._notificationIcon.MouseClick += notificationIcon_MouseClick;
            this._notificationIcon.MouseDoubleClick += notificationIcon_MouseDoubleClick;
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

        private void ShowBalloonTip(WorkDay today)
        {
            var balloonText = $"{"Start:",-16}\t{today.Start.ToHhmmss(),10}\n";
            balloonText += $"{"Estimated:",-16}\t{today.Estimated.ToHhmmss(),10}\n";
            balloonText += $"{"Elapsed:",-16}\t{today.Elapsed.ToHhmmss(),10}\n";
            balloonText += $"{"Remaining:",-16}\t{today.Remaining.ToHhmmss(),10}\n";
            _notificationIcon.ShowBalloonTip(BalloonTimeOut, Properties.Resources.ApplicationName, balloonText, System.Windows.Forms.ToolTipIcon.Info);
        }

        /// <summary>
        /// Handles the MouseClick event of the systrayIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void notificationIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            var today = (DataContext as MainViewModel)?.History.Today;

            // Check last balloon update to prevent it from flickering
            if (e.Button == System.Windows.Forms.MouseButtons.Right && today != null)
            {
                ShowBalloonTip(today);
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the systrayIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        private void notificationIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // The state change will trigger the state changed event and do everything else there
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Handles the PowerModeChanged event of the SystemEvents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Win32.PowerModeChangedEventArgs"/> instance containing the event data.</param>
        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            switch (e.Mode)
            {
                case Microsoft.Win32.PowerModes.Resume:
                    Load();
                    break;
                case Microsoft.Win32.PowerModes.Suspend:
                    SaveSilent();
                    break;
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
        }
    }
}