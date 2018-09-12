using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.ViewModels
{
    internal class StatusBarViewModel : ViewModelBase
    {
        public int DayCount { get; set; }
        public int Overall { get; set; }
        public int Intended { get; set; }
        public int Difference { get; set; }
    }
}