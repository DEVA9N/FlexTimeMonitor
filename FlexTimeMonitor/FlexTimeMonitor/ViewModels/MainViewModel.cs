using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A9N.FlexTimeMonitor.Windows;

namespace A9N.FlexTimeMonitor.ViewModels
{
    internal sealed class MainViewModel : ViewModelBase
    {
        public MainViewModel(MainWindow window)
        {
            Content = new WorkDayGridViewModel();
            Menu = new MenuViewModel(window);
            Status = new StatusBarViewModel();
        }

        public WorkDayGridViewModel Content { get; }
        public MenuViewModel Menu { get; }
        public StatusBarViewModel Status { get; }
    }
}
