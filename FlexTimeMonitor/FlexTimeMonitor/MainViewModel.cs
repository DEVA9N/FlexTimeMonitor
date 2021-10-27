using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Views;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor
{
    internal sealed class MainViewModel : ViewModel
    {
        private readonly WorkHistoryFile _historyFile;

        public bool SelectionPopupVisible { get; set; }
        public WorkHistory History { get; private set; }
        public MenuViewModel Menu { get; }
        public SelectionViewModel Selection { get; set; }

        public MainViewModel(MainView window)
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
            _historyFile.Save();
        }

    }
}
