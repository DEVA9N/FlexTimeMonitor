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
using A9N.FlexTimeMonitor.Helper;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        private RegistrySettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsWindow"/> class.
        /// </summary>
        public OptionsWindow()
        {
            InitializeComponent();

            settings = new RegistrySettings();

            checkBoxAutoStart.IsChecked = settings.AutoStart;
            textBoxWorkPeriod.Text = Properties.Settings.Default.WorkPeriod.ToString();
            textBoxBreakPeriod.Text = Properties.Settings.Default.BreakPeriod.ToString();
        }

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

            textBoxWorkPeriod.Foreground = parseWorkSuccess ? validResultBrush : invalidResultBrush;
            textBoxBreakPeriod.Foreground = parseBreakSuccess ? validResultBrush : invalidResultBrush;

            bool allValuesValid = parseWorkSuccess && parseBreakSuccess;// && parsePathSuccess;

            if (allValuesValid)
            {
                Properties.Settings.Default.WorkPeriod = resultWork;
                Properties.Settings.Default.BreakPeriod = resultBreak;
                Properties.Settings.Default.Save();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handles the Click event of the buttonOk control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            settings.AutoStart = checkBoxAutoStart.IsChecked ?? false;

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
    }
}
