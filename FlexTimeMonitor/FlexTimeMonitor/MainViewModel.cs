using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Views;

namespace A9N.FlexTimeMonitor
{
    internal sealed class MainViewModel : ViewModel
    {
        private readonly IWorkHistoryService _historyService;
        public bool SelectionPopupVisible { get; set; }
        public MenuViewModel Menu { get; }
        public WorkDayGridViewModel Grid { get; private set; }
        public SelectionViewModel Selection { get; set; }

        public MainViewModel(MenuViewModel menu, IWorkHistoryService historyService)
        {
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
            Menu = menu ?? throw new ArgumentNullException(nameof(menu));
        }

        internal void OpenHistory()
        {
            var items = _historyService.GetItems();

            Grid = new WorkDayGridViewModel(items);
        }

        internal void SaveHistory()
        {
            var data = Grid.Items.Select(i => i.ToWorkDayData());

            _historyService.SaveItems(data);
        }
    }
}
