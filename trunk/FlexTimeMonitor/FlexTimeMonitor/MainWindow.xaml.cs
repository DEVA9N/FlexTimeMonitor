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
        private const String ApplicationName = "Flex Time Monitor";
        private const String DefaultLogfileName = "flextimelog.xml";

        private WorkDay today;
        private WorkHistory history;
        private HistoryFile historyFile;
        private bool saveHistoryOnExit = true;

        private System.Windows.Forms.NotifyIcon systrayIcon;
        private TimeSpan balloonOpenTime = TimeSpan.Zero;
        private TimeSpan balloonTimeOut = new TimeSpan(0, 0, 3);

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
            this.systrayIcon.Icon = A9N.FlexTimeMonitor.Properties.Resources.Stopwatch;
            this.systrayIcon.Text = ApplicationName;
            this.systrayIcon.Visible = true;
            this.systrayIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(systrayIcon_MouseMove);
            this.systrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(systrayIcon_MouseClick);
        }

        /// <summary>
        /// Display from tray
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void systrayIcon_MouseClick(object sender, EventArgs e)
        {
            // The state change will trigger the state changed event and do everything else there
            this.WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Display status
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void systrayIcon_MouseMove(object sender, EventArgs e)
        {
            // Check last balloon update to prevent it from flickering
            if (today != null && (DateTime.Now.TimeOfDay - balloonOpenTime) > balloonTimeOut)
            {
                String balloonText = String.Format("{0,-16}\t{1,10}\n", "Start:", TimeSpanHelper.ToHhmmss(today.Start.TimeOfDay));
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Estimated:", TimeSpanHelper.ToHhmmss(today.Estimated));
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Elapsed:", TimeSpanHelper.ToHhmmss(today.Elapsed));
                balloonText += String.Format("{0,-16}\t{1,10}\n", "Remaining:", TimeSpanHelper.ToHhmmss(today.Remaining));
                systrayIcon.ShowBalloonTip(balloonTimeOut.Seconds, ApplicationName, balloonText, System.Windows.Forms.ToolTipIcon.Info);

                // Remember last open
                balloonOpenTime = DateTime.Now.TimeOfDay;
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Get history file name
        /// </summary>
        /// <returns>String.</returns>
        private String GetFileName()
        {
            // If there is no file name configured - create new default file name
            if (Settings.Default.LogfileName == String.Empty)
            {
                String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
                String appName = ApplicationName + "\\";
                Settings.Default.LogfileName = myDocuments + appName + DefaultLogfileName;
                Settings.Default.Save();
            }

            return Settings.Default.LogfileName;
        }

        private String GetVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                // This version matches the published version and is only available in the published application
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                // This return the AssemblyInfos' version which differs to the deployment version
                return System.Windows.Forms.Application.ProductVersion;
            }
            // Todo: Check if it a good idea to sync those numbers
        }
        #endregion

#if DEBUG
        /// <summary>
        /// Creates the sample history.
        /// </summary>
        /// <param name="history">The history.</param>
        /// <param name="count">The count.</param>
        private void CreateSampleHistory(WorkHistory history, int count)
        {
            if (history == null)
            {
                return;
            }

            history.Clear();

            for (int i = 0; i < count; i++)
            {
                DateTime date = new DateTime(1970, 1, 1, 9, 0, 21).AddDays(i);

                WorkDay day = new WorkDay();
                day.Start = date;
                day.End = date.AddHours(9);
                history.Add(day);
            }
        }
#endif

        #region History Access
        /// <summary>
        /// Opens the history.
        /// </summary>
        private void OpenHistory()
        {

            String historyPath = GetFileName();
            historyFile = new HistoryFile(historyPath);

            // Will try to open an existing history file. If none is found it will create a new one.
            // If reading or writing fails the user is informed and the application is shut down without writing data.
            try
            {
                history = historyFile.Load();

                // Get today from history - never get it somewhere else!
                today = history.GetToday();

                // This will make sure that the start is logged correctly even if the computer crashes
                historyFile.Save(history);

                dataGridWorkDays.ItemsSource = history;

                // Set focus on last item
                if (dataGridWorkDays.Items.Count > 0)
                {
                    dataGridWorkDays.UpdateLayout();
                    dataGridWorkDays.ScrollIntoView(dataGridWorkDays.Items[dataGridWorkDays.Items.Count - 1]);
                }
            }
            catch (Exception e)
            {
                String text = ApplicationName + " is unable to load the history file:\n";
                text += GetFileName() + "\n";
                text += "This surely should not happen but evidently it has!\n";
                text += "\n";
                text += "Now there are 3 options:\n";
                text += "\t- delete the history file and lose all your data or\n";
                text += "\t- manually fix the history file (see details below) or\n";
                text += "\t- file a bug report and try to convince the author to fix that file for you\n";
                text += "\n";
                text += e.Message + "\n";
                text += "\n";
                text += "Press OK to quit the application.";

                MessageBox.Show(text, "Unable load history file");

                // This will close the application but will also trigger the MainWindow's closing event!
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Saves the history.
        /// </summary>
        private void SaveHistory()
        {
            // Set end time and save object
            // Note that the history is not a valid object if it failed to load
            if (history != null)
            {
                today.End = DateTime.Now;

                historyFile.Save(history);
            }
        }

        /// <summary>
        /// Closes the history.
        /// </summary>
        private void CloseHistory()
        {

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

#if DEBUG
//            if (history != null && history.Count == 0)
//            {
//                CreateSampleHistory(history, 222);
//            }
#endif
        }

        /// <summary>
        /// This event is called when the main window is closed. It doesn't matter if it is closed
        /// by user or by exception. It will be called either way!
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs" /> instance containing the event data.</param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Remove systray icon
            systrayIcon.Visible = false;
            systrayIcon.Dispose();

            if (saveHistoryOnExit)
            {
                SaveHistory();
            }
            else
            {
                CloseHistory();
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
                this.systrayIcon.Visible = true;
            }
            if (WindowState == WindowState.Normal)
            {
                this.ShowInTaskbar = true;
                this.systrayIcon.Visible = false;
            }
        }

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
                foreach (WorkDay w in dataGridWorkDays.SelectedItems)
                {
                    timeOverall += w.Elapsed - Settings.Default.BreakPeriod;
                    timeIntended += Settings.Default.WorkPeriod;
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
            saveHistoryOnExit = false;
            Close();
        }

        /// <summary>
        /// Handles the 1 event of the MenuItemEditOptions_Click control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemEditOptions_Click_1(object sender, RoutedEventArgs e)
        {
            WindowOptions options = new WindowOptions();
            options.ShowDialog();
        }

        /// <summary>
        /// Handles the Click event of the MenuItemAbout control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            String newLine = Environment.NewLine;
            String caption = "About " + ApplicationName;

            String aboutText = "";
            aboutText += ApplicationName + newLine + newLine;
            aboutText += GetVersion() + newLine + newLine;
            aboutText += "Copyright © 2009-2012 Andre Janßen" + newLine + newLine;
            aboutText += "Visit http://a9n.de for further information";

            MessageBox.Show(aboutText, caption);
        }
        #endregion

    }
}