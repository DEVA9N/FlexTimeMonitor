using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowOptions" /> class.
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();

            textBoxWorkPeriod.Text = Properties.Settings.Default.WorkPeriod.ToString();
            textBoxBreakPeriod.Text = Properties.Settings.Default.BreakPeriod.ToString();
            //textBoxFile.Text = Properties.Settings.Default.LogfileName;
        }

        #region Private Methods
        /// <summary>
        /// Saves the values.
        /// </summary>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise</returns>
        private bool SaveValues()
        {
            SolidColorBrush validResultBrush = new SolidColorBrush(Colors.Black);
            SolidColorBrush invalidResultBrush = new SolidColorBrush(Colors.Red);
            TimeSpan resultWork = TimeSpan.Zero;
            TimeSpan resultBreak = TimeSpan.Zero;

            bool parseWorkSuccess = TimeSpan.TryParse(textBoxWorkPeriod.Text, out resultWork);
            bool parseBreakSuccess = TimeSpan.TryParse(textBoxBreakPeriod.Text, out resultBreak);
            //bool parsePathSuccess = System.IO.File.Exists(textBoxFile.Text);

            textBoxWorkPeriod.Foreground = parseWorkSuccess ? validResultBrush : invalidResultBrush;
            textBoxBreakPeriod.Foreground = parseBreakSuccess ? validResultBrush : invalidResultBrush;
            //textBoxFile.Foreground = parsePathSuccess ? validResultBrush : invalidResultBrush;

            bool allValuesValid = parseWorkSuccess && parseBreakSuccess;// && parsePathSuccess;

            if (allValuesValid)
            {
                Properties.Settings.Default.WorkPeriod = resultWork;
                Properties.Settings.Default.BreakPeriod = resultBreak;
                //Properties.Settings.Default.LogfileName = textBoxFile.Text;
                Properties.Settings.Default.Save();
                return true;
            }

            return false;
        }
        #endregion

        #region Button Events
        /// <summary>
        /// Handles the Click event of the buttonOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (SaveValues())
            {
                Close();
            }
        }

        /// <summary>
        /// Handles the Click event of the buttonCancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

    }
}
