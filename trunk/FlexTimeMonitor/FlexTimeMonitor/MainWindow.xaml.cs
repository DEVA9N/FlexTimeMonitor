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
        private const String ApplicationName = "FlexTimeMonitor";
        private const String DefaultLogfileName = "flextimelog.xml";

        private bool saveHistoryOnExit = false;
        private WorkDay today;
        private WorkHistory history;
        private HistoryFile historyFile;
        private System.Windows.Forms.NotifyIcon systrayIcon;

        public MainWindow()
        {
            InitializeComponent();

            InitializeSystrayIcon();

            // Note this will only be visible in the deployed version - not during debugging
            AddVersionString();

            historyFile = new HistoryFile(GetFileName());

            // TODO: replace Load and Save
            // The current implementation is dangerous. It overwrites files when first installed (config path == ""?)

            // IDEA:
            // -Show hint with default file name / or a "DefaultButton"
            // -Create a max log period in months. Delete older entries automatically
            // -Automatically create weekly backups and monthly ones.
            // -Save configuration in default path (not that cryptic path used by the installed application)

            // Will try to open an existing history file. If none is found it will create a new one.
            // If reading or writing fails the user is informed and the application is shut down without writing data.
            try
            {
                history = historyFile.Load();

                // This will bind the current history to the datagrid
                dataGridWorkDays.ItemsSource = history;

                // Get today from history - never get it somewhere else!
                today = history.GetToday();

                // This will make sure that the start is logged correctly even if the computer crashes
                historyFile.Save(history);

                saveHistoryOnExit = true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Unable to access file");

                // This will close the application but will also trigger the MainWindow's closing event!
                Application.Current.Shutdown();
            }
        }




        #region Systray icon

        private void InitializeSystrayIcon()
        {
            this.systrayIcon = new System.Windows.Forms.NotifyIcon();
            this.systrayIcon.Icon = A9N.FlexTimeMonitor.Properties.Resources.Icon_16;
            this.systrayIcon.Text = ApplicationName;
            this.systrayIcon.Visible = true;
            this.systrayIcon.MouseMove += new System.Windows.Forms.MouseEventHandler(systrayIcon_MouseMove);
            this.systrayIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(systrayIcon_MouseClick);
        }

        /// <summary>
        /// Display from tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void systrayIcon_MouseClick(object sender, EventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Display status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void systrayIcon_MouseMove(object sender, EventArgs e)
        {
            systrayIcon.BalloonTipText = "Start: " + today.Start.ToString("t");
            systrayIcon.BalloonTipText += "\nEstimated: " + today.Estimated.ToString("t");
            systrayIcon.BalloonTipText += "\nElapsed: " + TimeSpanTotalToString(today.Elapsed);
            systrayIcon.BalloonTipText += "\nRemaining: " + TimeSpanTotalToString(today.Remaining);
            systrayIcon.ShowBalloonTip(10);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Adds a version string to the main window title.
        /// NOTE: The string will only be visible in the deployed application and
        /// not during development.
        /// </summary>
        private void AddVersionString()
        {
            //if (ApplicationDeployment.IsNetworkDeployed)
            //{
            //    Version currentVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
            //    Title += " - " + currentVersion.ToString();
            //}
        }

        /// <summary>
        /// Get history file name
        /// </summary>
        /// <returns></returns>
        private String GetFileName()
        {
            // There is no file name configured - create new default file name
            if (Settings.Default.LogfileName == String.Empty)
            {
                String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
                String appName = ApplicationName + "\\";
                Settings.Default.LogfileName = myDocuments + appName + DefaultLogfileName;
                Settings.Default.Save();
            }

            return Settings.Default.LogfileName;
        }



        /// <summary>
        /// Converts the ToString result to 
        /// 9:47 format. 
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private static String TimeSpanTotalToString(TimeSpan t)
        {
            if (t != null)
            {
                bool isNegative = t.Ticks < 0;
                String prefix = isNegative ? "-" : "";

                return prefix + t.ToString(@"hh\:mm");
            }
            return "";
        }

        #endregion

        #region Window Events

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Remove systray icon
            systrayIcon.Visible = false;
            systrayIcon.Dispose();

            // If the history file data was invalid or some other problem occured it may be 
            // neccessary to quit without saving - so that's the default.
            if (saveHistoryOnExit)
            {
                // Set end time and save object
                if (today != null)
                {
                    today.End = DateTime.Now;
                }

                historyFile.Save(history);
            }
        }

        /// <summary>
        /// Minimize to tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        /// <summary>
        /// Display selection results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            statusBarOverallValue.Text = TimeSpanTotalToString(timeOverall);
            statusBarIntendedValue.Text = TimeSpanTotalToString(timeIntended);
            statusBarDifferenceValue.Text = TimeSpanTotalToString(timeOverall - timeIntended);
        }

        #endregion

        #region Menu item events

        private void menuItemFile_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemEditOptions_Click_1(object sender, RoutedEventArgs e)
        {
            WindowOptions options = new WindowOptions();
            options.ShowDialog();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {

            String aboutText = "   " + Title + "\n\n";
            aboutText += "©2009 Andre Janßen - http://a9n.de";

            MessageBox.Show(aboutText, ApplicationName);
        }

        #endregion


    }
}