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
        private readonly WorkHistoryFile _historyFile;

        public WorkHistory History { get; private set; }
        public MenuViewModel Menu { get; }
        public StatusBarViewModel Status { get; }

        public MainViewModel(MainWindow window)
        {
            _historyFile = new WorkHistoryFile();

            Status = new StatusBarViewModel();
            Menu = new MenuViewModel(window);
            OpenHistory();
        }

        internal void OpenHistory()
        {
            _historyFile.Load();
            History = _historyFile.History;
        }

        internal void SaveHistory()
        {
            _historyFile?.Save();
        }

    }
}
