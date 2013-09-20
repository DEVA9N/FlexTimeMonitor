using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkHistoryFile historyFile;
        private bool autoSaveHistory = true;
        private System.Windows.Forms.NotifyIcon systrayIcon;
        private const int BalloonTimeOut = 3000;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            InitializeSystrayIcon();
        }

        #region Systray icon
        /// <summary>
        /// Initializes the systray icon.
        /// </summary>
        private void InitializeSystrayIcon()
        {
            this.systrayIcon = new System.Windows.Forms.NotifyIcon();
            this.systrayIcon.Icon = Properties.Resources.Stopwatch;
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
                String balloonText = String.Format("{0,-16}\t{1,10}\n", "Start:", TimeSpanHelper.ToHhmmss(today.Start));
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Estimated:", TimeSpanHelper.ToHhmmss(today.Estimated));
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Elapsed:", TimeSpanHelper.ToHhmmss(today.Elapsed));
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Remaining:", TimeSpanHelper.ToHhmmss(today.Remaining));
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
            historyFile = new WorkHistoryFile();

            // Will try to open an existing history file. If none is found it will create a new one.
            // If reading or writing fails the user is informed and the application is shut down without writing data.
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
        private void SaveHistory()
        {
            // Set end time and save object
            // Note that the history is not a valid object if it failed to load
            if (historyFile != null)
            {
                try
                {
                    historyFile.Save();
                }
                catch (Exception e)
                {
                    MessageBox.Show(this, e.Message, Properties.Resources.ApplicationName);
                }
            }
        }
        #endregion

        #region Window Events
        /// <summary>
        /// This event is called directly when the window is loaded and should be used
        /// to do post initialization stuff
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            OpenHistory();
        }

        /// <summary>
        /// This event is called when the main window is closed. It doesn't matter if it is closed
        /// by user or by exception. It will be called either way!
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (autoSaveHistory)
            {
                SaveHistory();
            }
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
            if (WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
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
            statusBarOverallValue.Text = TimeSpanHelper.ToHhmmss(timeOverall);
            statusBarIntendedValue.Text = TimeSpanHelper.ToHhmmss(timeIntended);
            statusBarDifferenceValue.Text = TimeSpanHelper.ToHhmmss(timeOverall - timeIntended);
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
        /// Handles the Click event of the MenuItemQuitWithoutSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemQuitWithoutSave_Click(object sender, RoutedEventArgs e)
        {
            // Disable autosave
            autoSaveHistory = false;

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

            about.ShowDialog();
        }
        #endregion

    }
}