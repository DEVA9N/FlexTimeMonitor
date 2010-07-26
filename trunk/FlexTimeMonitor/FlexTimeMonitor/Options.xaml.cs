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
    public partial class WindowOptions : Window
    {
        public WindowOptions()
        {
            InitializeComponent();

            textBoxWorkPeriod.Text = Properties.Settings.Default.WorkPeriod.ToString();
            textBoxBreakPeriod.Text = Properties.Settings.Default.BreakPeriod.ToString();
            textBoxFile.Text = Properties.Settings.Default.LogfileName;
        }

        private void buttonOk_Click(object sender, RoutedEventArgs e)
        {
            if (SaveValues())
            {
                Close();
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private bool SaveValues()
        {
            SolidColorBrush validResultBrush = new SolidColorBrush(Colors.Black);
            SolidColorBrush invalidResultBrush = new SolidColorBrush(Colors.Red);
            TimeSpan resultWork = TimeSpan.Zero;
            TimeSpan resultBreak = TimeSpan.Zero;

            bool parseWorkSuccess = TimeSpan.TryParse(textBoxWorkPeriod.Text, out resultWork);
            bool parseBreakSuccess = TimeSpan.TryParse(textBoxBreakPeriod.Text, out resultBreak);
            bool parsePathSuccess = System.IO.File.Exists(textBoxFile.Text);

            textBoxWorkPeriod.Foreground = parseWorkSuccess ? validResultBrush : invalidResultBrush;
            textBoxBreakPeriod.Foreground = parseBreakSuccess ? validResultBrush : invalidResultBrush;
            textBoxFile.Foreground = parsePathSuccess ? validResultBrush : invalidResultBrush;

            bool allValuesValid = parseWorkSuccess && parseBreakSuccess;// && parsePathSuccess;

            if (allValuesValid)
            {
                Properties.Settings.Default.WorkPeriod = resultWork;
                Properties.Settings.Default.BreakPeriod = resultBreak;
                Properties.Settings.Default.LogfileName = textBoxFile.Text;
                Properties.Settings.Default.Save();
                return true;
            }

            return false;
        }

        private void buttonFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();

            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (System.IO.File.Exists(ofd.FileName))
                {
                    textBoxFile.Text = ofd.FileName;
                    Properties.Settings.Default.LogfileName = ofd.FileName;
                }
            }
        }

    }
}
