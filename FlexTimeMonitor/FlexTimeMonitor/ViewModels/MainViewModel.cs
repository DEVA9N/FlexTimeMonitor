using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using A9N.FlexTimeMonitor.Windows;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.ViewModels
{
    internal sealed class MainViewModel : ViewModelBase
    {
        private readonly WorkHistoryFile _historyFile;

        public bool SelectionPopupVisible { get; set; }
        public Work.WorkHistory History { get; private set; }
        public MenuViewModel Menu { get; }
        public SelectionViewModel Selection { get; set; }

        public MainViewModel(MainWindow window)
        {
            _historyFile = new WorkHistoryFile();

            Menu = new MenuViewModel(window);
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
