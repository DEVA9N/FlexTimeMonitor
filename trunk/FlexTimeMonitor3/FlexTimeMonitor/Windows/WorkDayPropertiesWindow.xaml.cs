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

namespace A9N.FlexTimeMonitor.Windows
{
    /// <summary>
    /// Interaction logic for DayPropertiesWindow.xaml
    /// </summary>
    public partial class WorkDayPropertiesWindow : Window
    {
        private WorkDay _workDay;

        public WorkDay WorkDay
        {
            get { return _workDay; }
            set {
                _workDay = value;

                if (_workDay != null)
                {
                    Populate(_workDay);
                }
                else
                {
                    Clear();
                }
            }
        }

        private void Populate(FlexTimeMonitor.WorkDay _workDay)
        {
            this.DayLabel.Content = _workDay.Date.ToString();
        }

        private void Clear()
        {
            this.DayLabel.Content = String.Empty;
        }

        public WorkDayPropertiesWindow()
        {
            InitializeComponent();
        }
    }
}
