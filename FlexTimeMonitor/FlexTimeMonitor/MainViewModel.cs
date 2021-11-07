using System;
using System.Collections.Generic;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Views;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor
{
    internal sealed class MainViewModel : ViewModel
    {
        private readonly IWorkHistoryService _historyService;
        public bool SelectionPopupVisible { get; set; }
        public MenuViewModel Menu { get; }
        public SelectionViewModel Selection { get; set; }

        public MainViewModel(MainView window, IWorkHistoryService historyService)
        {
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
            Menu = new MenuViewModel(window);
        }

        internal void OpenHistory()
        {
            var historyData = _historyService.GetHistory();

        }

        internal void SaveHistory()
        {

            _historyService.SaveHistory();
        }

    }


}
