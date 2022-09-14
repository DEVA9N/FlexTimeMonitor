using System;
using System.Collections.Generic;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Views;

namespace A9N.FlexTimeMonitor
{
    internal sealed class MainViewModel : ViewModel
    {
        private readonly IWorkHistoryService _historyService;

        public MenuViewModel Menu { get; }

        public WorkDayGridViewModel Grid { get; private set; }

        public String BalloonText => NotificationTextCreator.TryCreateText(Grid.Today?.ToEntity());

        public MainViewModel(MenuViewModel menu, IWorkHistoryService historyService)
        {
            _historyService = historyService ?? throw new ArgumentNullException(nameof(historyService));
            Menu = menu ?? throw new ArgumentNullException(nameof(menu));
        }

        internal void OpenHistory()
        {
            var items = _historyService.GetItems().ToList();

            if (!ContainsToday(items))
            {
                var today = CreateToday();
                items.Add(today);

                _historyService.SaveItems(items);
            }

            Grid = new WorkDayGridViewModel(items);
        }

        internal void SaveHistory()
        {
            var data = Grid.Items.Select(i => i.ToEntity());

            _historyService.SaveItems(data);
        }

        private static bool ContainsToday(IEnumerable<WorkDayEntity> items)
        {
            return items.Any(i => i.Start.Date == DateTime.Today.Date);
        }

        private static WorkDayEntity CreateToday()
        {
            return new WorkDayEntity { Start = DateTime.Now, End = DateTime.Now };
        }

    }
}
