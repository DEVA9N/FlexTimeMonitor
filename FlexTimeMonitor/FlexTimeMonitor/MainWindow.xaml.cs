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
        private WorkDay today;
        private WorkHistory history;
        private System.Windows.Forms.NotifyIcon systrayIcon;

        public MainWindow()
        {
            InitializeComponent();

            InitializeSystrayIcon();

            // Note this will only be visible in the deployed version - not during debugging
            AddVersionString();

            // TODO: replace Load and Save
            // The current implementation is dangerous. It overwrites files when first installed (config path == ""?)
            // 
            // New policy: Policy warn on load, solve on close.
            // 1. First start: check if file name is configured (which is not the case on the real first start)
            // 2. Create default file name and file if it does not already exist
            // 3. If that file exists - use it - never overwrite a file without impoting it's content first
            // After this point:
            // The file name != String.Empty. If so do not take further file check actions. The user is to be informed
            // about file issues via a Messagebox and has to resolve the problems on It's own.

            // IDEA:
            // -Show hint with default file name / or a "DefaultButton"
            // -Create a max log period in months. Delete older entries automatically
            // -Automatically create weekly backups and monthly ones.
            // -Save configuration in default path (not that cryptic path used by the installed application)
            LoadHistory();

            // Get today from history - never get it somewhere else!
            today = history.GetToday();

            // This will make sure that the start is logged correctly even if the computer crashes
            SaveHistory();
        }


        private String CreateDefaultFileName()
        {
            // Create default filename
            String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
            String appName = Settings.Default.ApplicationName + "\\";

            return myDocuments + appName + "timelog.xml";
        }

        private void CreateBackup()
        { 
        
        }

        #region Systray icon

        private void InitializeSystrayIcon()
        {
            this.systrayIcon = new System.Windows.Forms.NotifyIcon();
            this.systrayIcon.Icon = A9N.FlexTimeMonitor.Properties.Resources.Icon_16;
            this.systrayIcon.Text = Settings.Default.ApplicationName;
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
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version currentVersion = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                Title += " - " + currentVersion.ToString();
            }
        }

        /// <summary>
        /// The major goal is still to get the application running savely without user interaction.
        /// 
        /// </summary>
        private void LoadHistory()
        {
            String fileName = Settings.Default.LogfileName;

            // There is no fileName configured - create new default fileName
            if (fileName == String.Empty)
            {
                Settings.Default.LogfileName = CreateDefaultFileName();
                Settings.Default.Save();
                fileName = Settings.Default.LogfileName;
            }

            // At this point there is a valid filename but unknown if that file exists
            if (File.Exists(fileName) == false)
            {
                // Create path

                // Create file

            }

            // The file exists, but it's inaccessible. There is a major problem which can not be resolved by this application 
            // Note that the file name and path may also be user defined.
            if (FileUtils.TryOpen(fileName) == false)
            {
                String error = "";
                error += "The application is unable to access the log file\n ";
                error += Settings.Default.LogfileName + "\n";
                error += "Please check if the file name is valid and the destination is not write protected";

                MessageBox.Show(error, "Unable to access file");

                // TODO: make sure it stops immediatly
                Application.Current.Shutdown();
            }

            // Load file content
            try
            {
                history = XmlManager.Read<WorkHistory>(Settings.Default.LogfileName);
            }
            catch (Exception e)
            {
                // NOTE: this is a crucial spot! The file is existant but it is unkown if it contains data or if that data is valid.
                // That's why it is important to create a backup in case of an error during deserialization.
                System.Diagnostics.Debug.WriteLine("Unable to load history file " + Settings.Default.LogfileName + "\n" + e);
                CreateBackup();
            }

            // These things should be done even and espacially if the load fails. 
            // This step will erase the previous history entrys in the case that the file deserialization threw an exception.
            if (history == null)
            {
                history = new WorkHistory();
            }

            // This will bind the current history to the datagrid
            dataGridWorkDays.ItemsSource = history;
        }


        /// <summary>
        /// Save history to file.
        /// </summary>
        private void SaveHistory()
        {
            try
            {
                // Do all the file checking stuff again...

                // Make the file ready to be saved - this espacially involves the path which must be present
                if (File.Exists(Settings.Default.LogfileName) == false)
                {
                    String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\";
                    String appName = Settings.Default.ApplicationName + "\\";
                    String path = myDocuments + appName;

                    if (Directory.Exists(path) == false)
                    {
                        Directory.CreateDirectory(path);
                    }

                    Settings.Default.LogfileName = path + "timelog.xml";
                    Settings.Default.Save();
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to provide prepared history file" + Settings.Default.LogfileName + "\n" + e);
            }

            try
            {
                XmlManager.Write(Properties.Settings.Default.LogfileName, history);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to save history from file " + Settings.Default.LogfileName + "\n" + e);
            }
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

            // Set end time and save object
            if (today != null)
            {
                today.End = DateTime.Now;
            }

            SaveHistory();
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

            MessageBox.Show(aboutText, Settings.Default.ApplicationName);
        }

        #endregion


    }
}