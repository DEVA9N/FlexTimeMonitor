using System;
using System.Windows;
using System.Windows.Controls;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkHistoryFile historyFile;
        private System.Windows.Forms.NotifyIcon systrayIcon;
        private const int BalloonTimeOut = 3000;

        /// <summary>
        /// Gets or sets a value indicating whether to show the save dialog when the program is closing.
        /// </summary>
        /// <value><c>true</c> if the save dialog should be shown; otherwise, <c>false</c>.</value>
        internal bool ShowSaveDialog { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.ShowSaveDialog = true;

            InitializeSystrayIcon();

            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
        }

        #region Systray icon
        /// <summary>
        /// Initializes the systray icon.
        /// </summary>
        private void InitializeSystrayIcon()
        {
            this.systrayIcon = new System.Windows.Forms.NotifyIcon();
            this.systrayIcon.Icon = Properties.Resources.ApplicationIconLight;
            this.systrayIcon.Text = Properties.Resources.ApplicationName;
            this.systrayIcon.Visible = true;
            this.systrayIcon.MouseClick += systrayIcon_MouseClick;
            this.systrayIcon.MouseDoubleClick += systrayIcon_MouseDoubleClick;
        }

        /// <summary>
        /// Handles the MouseClick event of the systrayIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void systrayIcon_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (historyFile == null)
            {
                // Nothing todo here
                return;
            }

            var today = historyFile.History.Today;

            // Check last balloon update to prevent it from flickering
            if (e.Button == System.Windows.Forms.MouseButtons.Right && today != null)
            {
                String balloonText = String.Format("{0,-16}\t{1,10}\n", "Start:", today.Start.ToHhmmss());
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Estimated:", today.Estimated.ToHhmmss());
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Elapsed:", today.Elapsed.ToHhmmss());
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Remaining:", today.Remaining.ToHhmmss());
                systrayIcon.ShowBalloonTip(BalloonTimeOut, Properties.Resources.ApplicationName, balloonText, System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the systrayIcon control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
        void systrayIcon_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // The state change will trigger the state changed event and do everything else there
            this.WindowState = WindowState.Normal;

            try
            {
                this.dataGridWorkDays.Items.Refresh();
            }
            catch (InvalidOperationException)
            {
                // This exception occurs if the cell of an edited item has not been left before putting this app to tray.
            }
        }
        #endregion

        #region History Access
        /// <summary>
        /// Opens the history.
        /// </summary>
        private void OpenHistory()
        {
            // The application decides whether it can save or not by checking if the historyFile is null. This should
            // prevent the application from overwriting an existing file with new (not properly loaded) data.
            if (historyFile == null)
            {
                historyFile = new WorkHistoryFile();
            }

            // Will try to open an existing history file. If none is found it will create a new one.
            // If reading or writing fails the user is informed.
            try
            {
                historyFile.Load();

                dataGridWorkDays.ItemsSource = historyFile.History;

                // Set focus on last item
                if (dataGridWorkDays.Items.Count > 0)
                {
                    dataGridWorkDays.UpdateLayout();
                    dataGridWorkDays.ScrollIntoView(dataGridWorkDays.Items[dataGridWorkDays.Items.Count - 1]);
                }
            }
            catch (Exception e)
            {
                var backupFileName = historyFile.CreateBackup();
                var text = String.Format(Properties.Resources.Status_ErrorLoadingHistory, backupFileName, e);

                MessageBox.Show(this, text, Properties.Resources.ApplicationName);
            }
        }

        /// <summary>
        /// Saves the history.
        /// </summary>
        internal void SaveHistory()
        {
            // Set end time and save object
            // Note that the history is first available after the window has been loaded once
            if (historyFile != null)
            {
                try
                {
                    // Commit edits of opened cells that have not yet been committed (by leaving the cell or pressing "enter").
                    this.dataGridWorkDays.CommitEdit(DataGridEditingUnit.Row, true);

                    historyFile.Save();
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
        }
        #endregion

        /// <summary>
        /// Updates the settings.
        /// </summary>
        private static void UpdateSettings()
        {
            if (Properties.Settings.Default.UpdateSettings)
            {
                Properties.Settings.Default.Upgrade();
                // Custom user variable that triggers the update process (true by default)
                Properties.Settings.Default.UpdateSettings = false;
                Properties.Settings.Default.Save();
            }
        }


        #region Window Events
        /// <summary>
        /// Handles the PowerModeChanged event of the SystemEvents control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Win32.PowerModeChangedEventArgs"/> instance containing the event data.</param>
        void SystemEvents_PowerModeChanged(object sender, Microsoft.Win32.PowerModeChangedEventArgs e)
        {
            if (historyFile != null)
            {
                switch (e.Mode)
                {
                    case Microsoft.Win32.PowerModes.Resume:
                        this.OpenHistory();
                        break;
                    case Microsoft.Win32.PowerModes.Suspend:
                        this.SaveHistory();
                        break;
                }
            }
        }

        /// <summary>
        /// This event is called directly when the window is loaded and should be used
        /// to do post initialization stuff
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateSettings();

            OpenHistory();
        }

        /// <summary>
        /// Handles Minimize to tray and Restore Window
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                this.ShowInTaskbar = false;
            }
            if (WindowState != WindowState.Minimized)
            {
                this.ShowInTaskbar = true;
            }
        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ShowSaveDialog)
            {
                MessageBoxResult result = MessageBox.Show(Properties.Resources.Message_Save_Text, Properties.Resources.Message_Save_Title, MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveHistory();
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
        #endregion

        #region DataGrid Events
        /// <summary>
        /// Display selection results
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        private void dataGridWorkDays_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TimeSpan timeOverall = TimeSpan.Zero;
            TimeSpan timeIntended = TimeSpan.Zero;

            try
            {
                foreach (Object item in dataGridWorkDays.SelectedItems)
                {
                    if (item is WorkDay)
                    {
                        timeOverall += ((WorkDay)item).Elapsed - Settings.Default.BreakPeriod;
                        timeIntended += Settings.Default.WorkPeriod;
                    }
                }
            }
            catch (InvalidCastException x)
            {
                System.Diagnostics.Debug.WriteLine("Unsupported new-line-select in datagridview" + x);
            }

            // Display results in status bar
            statusBarDayCountValue.Text = dataGridWorkDays.SelectedItems.Count.ToString();
            statusBarOverallValue.Text = TimeSpanExtension.ToHhmmss(timeOverall);
            statusBarIntendedValue.Text = TimeSpanExtension.ToHhmmss(timeIntended);
            statusBarDifferenceValue.Text = TimeSpanExtension.ToHhmmss(timeOverall - timeIntended);
        }
        #endregion

        #region Menu item events
        /// <summary>
        /// Handles the Click event of the MenuItemSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveHistory();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemQuit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Handles the 1 event of the MenuItemEditOptions_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemEditOptions_Click_1(object sender, RoutedEventArgs e)
        {
            var options = new OptionsWindow();
            options.Owner = this;
            options.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            var about = new AboutWindow();
            about.Owner = this;

            about.ShowDialog();
        }
        #endregion

    }
}