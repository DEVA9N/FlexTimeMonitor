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
using A9N.FlexTimeMonitor.Controls;
using A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems;
using A9N.FlexTimeMonitor.Controls.DetailViewControls;
using System.Collections;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Forms.NotifyIcon systrayIcon;
        private const int BalloonTimeOut = 3000;

        public WorkHistory History { get; private set; }

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

            this.History = new WorkHistory();

            this.ShowSaveDialog = true;

            InitializeSystrayIcon();

            InitializeTree();
        }

        private void InitializeTree()
        {
            //this.historyTreeView.SelectedItemChanged += HistoryTreeView_SelectedItemChanged;
        }

        #region Systray icon
        /// <summary>
        /// Initializes the systray icon.
        /// </summary>
        private void InitializeSystrayIcon()
        {
            this.systrayIcon = new System.Windows.Forms.NotifyIcon();
            //this.systrayIcon.Icon = GetApplicationIcon();
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
            var today = History.Today;

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
                //this.datkaGridWorkDays.Items.Refresh();
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
            try
            {
                this.History.Load();

                historyTree.DataContext = new HistoryTreeViewModel(this.History);
            }
            catch (Exception e)
            {
                var text = String.Format(Properties.Resources.Status_ErrorLoadingHistory, e);

                MessageBox.Show(this, text, Properties.Resources.ApplicationName);
            }
        }

        /// <summary>
        /// Saves the history.
        /// </summary>
        internal void SaveHistory()
        {
            try
            {
                // Commit edits of opened cells that have not yet been committed (by leaving the cell or pressing "enter").
                //this.dataGridWorkDays.CommitEdit(DataGridEditingUnit.Row, true);

                this.History.Save();
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
        #endregion

        /// <summary>
        ///  Upgrade settings from the previous installation
        /// </summary>
        private static void UpgradeSettings()
        {
            try
            {
                if (Settings.Default.UpdateSettings)
                {
                    Settings.Default.Upgrade();
                    // Custom user variable that triggers the update process (true by default)
                    Settings.Default.UpdateSettings = false;
                    Settings.Default.Save();
                }
            }
            catch (Exception)
            {
                // TODO: Log error
            }
        }

        /// <summary>
        /// Handles the SelectedItemChanged event of the SystemEvents control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void HistoryTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.detailPanel.Children.Clear();

            //if (e.NewValue is YearViewModel)
            //{
            //    var detailView = new YearOverview();
            //    detailView.History = ((YearViewModel)e.NewValue).History;

            //    this.detailPanel.Children.Add(detailView);

            //}
            //else if (e.NewValue is MonthViewModel)
            //{
            //    var detailView = new MonthOverview();
            //    detailView.History = ((MonthViewModel)e.NewValue).Days;

            //    this.detailPanel.Children.Add(detailView);
            //}
        }


        #region Window Events

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
                    this.OpenHistory();
                    break;
                case Microsoft.Win32.PowerModes.Suspend:
                    this.SaveHistory();
                    break;
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
            UpgradeSettings();

            OpenHistory();

            // The power mode changes will to save / load the file
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
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