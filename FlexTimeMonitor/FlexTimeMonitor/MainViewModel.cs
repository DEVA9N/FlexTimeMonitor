using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Extensions;
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
        public String BalloonText => CreateBalloonText(Grid?.Today);

        private static String CreateBalloonText(WorkDayGridItemViewModel today)
        {
            if (today == null)
            {
                return string.Empty;
            }

            var balloonText = $"{"Start:",-16}\t{today.Start.ToHhmmss(),10}\n";
            balloonText += $"{"Estimated:",-16}\t{today.Estimated.ToHhmmss(),10}\n";
            balloonText += $"{"Elapsed:",-16}\t{today.Elapsed.ToHhmmss(),10}\n";
            balloonText += $"{"Remaining:",-16}\t{today.Remaining.ToHhmmss(),10}\n";

            return balloonText;
        }

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
