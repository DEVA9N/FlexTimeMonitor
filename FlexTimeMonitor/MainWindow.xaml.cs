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
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Controls;
using A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems;
using A9N.FlexTimeMonitor.Controls.DetailViewControls;
using System.Collections;
using A9N.FlexTimeMonitor.Helper;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal FTMData Data { get; private set; }

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

            String myDocuments = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Properties.Resources.ApplicationName);

            this.Data = new FTMData(myDocuments);

            this.ShowSaveDialog = true;

            InitializeTree();
        }

        private void InitializeTree()
        {
            this.historyTree.SelectedItemChanged += HistoryTree_SelectedItemChanged;
        }

        private void LoadData()
        {
            try
            {
                this.Data.Load();

                this.historyTree.DataContext = new HistoryTreeViewModel(this.Data.WorkDays);
                this.taskList.DataContext = new TaskListViewModel(this.Data.Tasks);
            }
            catch (Exception e)
            {
                var text = String.Format(Properties.Resources.Status_ErrorLoadingHistory, e);

                MessageBox.Show(this, text, Properties.Resources.ApplicationName);
            }
        }

        private void SaveData()
        {
            try
            {
                // Commit edits of opened cells that have not yet been committed (by leaving the cell or pressing "enter").
                //this.dataGridWorkDays.CommitEdit(DataGridEditingUnit.Row, true);

                this.Data.Save();
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
                    LoadData();
                    break;
                case Microsoft.Win32.PowerModes.Suspend:
                    SaveData();
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

            LoadData();

            // The power mode changes will to save / load the file
            Microsoft.Win32.SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
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
                        SaveData();
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

        /// <summary>
        /// Handles the SelectedItemChanged event of the SystemEvents control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        void HistoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            this.detailPanel.Children.Clear();

            if (e.NewValue is YearViewModel)
            {
                var detailView = new YearOverview();
                detailView.History = ((YearViewModel)e.NewValue).Days;

                this.detailPanel.Children.Add(detailView);
            }
            else if (e.NewValue is MonthViewModel)
            {
                var detailView = new MonthOverview();
                detailView.History = ((MonthViewModel)e.NewValue).Days;

                this.detailPanel.Children.Add(detailView);
            }
        }

        #region Menu item events
        /// <summary>
        /// Handles the Click event of the MenuItemSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
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