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
        private const String ApplicationName = "Flex Time Monitor";
        private const String DefaultLogfileName = "flextimelog.xml";

        private bool saveHistoryOnExit = false;
        private WorkDay today;
        private WorkHistory history;
        private HistoryFile historyFile;
        private System.Windows.Forms.NotifyIcon systrayIcon;

        public MainWindow()
        {
            EnforceSingleInstance();

            InitializeComponent();

            UpdateConfiguration();

            historyFile = new HistoryFile(GetFileName());

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
                    dataGridWorkDays.ScrollIntoView(dataGridWorkDays.Items[dataGridWorkDays.Items.Count - 1]);
                }

                // This is last since it relys on today (after history.GetToday) and a working application
                InitializeSystrayIcon();

                // Application is running smoothly so data can be saved on exit
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
            systrayIcon.BalloonTipText = "Start:\t\t" + TimeSpanHelper.TimeSpanToString(today.Start.TimeOfDay);
            systrayIcon.BalloonTipText += "\nEstimated:\t" + TimeSpanHelper.TimeSpanToString(today.Estimated);
            systrayIcon.BalloonTipText += "\nElapsed:\t" + TimeSpanHelper.TimeSpanToString(today.Elapsed);
            systrayIcon.BalloonTipText += "\nRemaining:\t" + TimeSpanHelper.TimeSpanToString(today.Remaining);
            systrayIcon.ShowBalloonTip(10);
        }

        #endregion

        #region Helper methods

        /// <summary>
        /// Multiple instance are useless and therefore forbidden.
        /// </summary>
        private void EnforceSingleInstance()
        {
            bool isFirstInstance;

            System.Threading.Mutex firstInstanceMutex = new System.Threading.Mutex(true, ApplicationName, out isFirstInstance);

            if (isFirstInstance == false)
            {
                saveHistoryOnExit = false;
                MessageBox.Show("Flex Time Monitor is already running.", "Error");
                Application.Current.Shutdown();
            }
        }

        /// <summary>
        /// Updates users' application configuration to be used in this version
        /// Once this is done this Method does nothing
        /// </summary>
        private void UpdateConfiguration()
        {
            // TODO: enter code here
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

        #region Window Events

        /// <summary>
        /// This event is called when the main window is closed. It doesn't matter if it is closed
        /// by user or by exception. It will be called either way!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            statusBarOverallValue.Text = TimeSpanHelper.TimeSpanToString(timeOverall);
            statusBarIntendedValue.Text = TimeSpanHelper.TimeSpanToString(timeIntended);
            statusBarDifferenceValue.Text = TimeSpanHelper.TimeSpanToString(timeOverall - timeIntended);
        }

        #endregion

        #region Menu item events

        private void MenuItemQuit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItemQuitWithoutSave_Click(object sender, RoutedEventArgs e)
        {
            saveHistoryOnExit = false;
            Close();
        }

        private void MenuItemEditOptions_Click_1(object sender, RoutedEventArgs e)
        {
            WindowOptions options = new WindowOptions();
            options.ShowDialog();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            String newLine = Environment.NewLine;
            String caption = "About " + ApplicationName;

            String aboutText = "";
            aboutText += ApplicationName + newLine + newLine;
            aboutText += GetVersion() + newLine + newLine;
            aboutText += "Copyright © 2009-2010 Andre Janßen" + newLine + newLine;
            aboutText += "Visit http://a9n.de for further information";

            MessageBox.Show(aboutText, caption);
        }

        #endregion

    }
}