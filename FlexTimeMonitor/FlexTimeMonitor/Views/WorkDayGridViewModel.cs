using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using A9N.FlexTimeMonitor.Entities;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.Controls
{
    internal class WorkDayGridViewModel : ViewModel
    {
        public ObservableCollection<WorkDayGridItemViewModel> Items { get; }

        public WorkDayGridItemViewModel Today { get; }

        public WorkDayGridViewModel(IEnumerable<WorkDayData> items)
        {
            var viewModels = from item in items
                             select new WorkDayGridItemViewModel(item);

            Items = new ObservableCollection<WorkDayGridItemViewModel>(viewModels);

            Today = GetToday();
        }

        private WorkDayGridItemViewModel FindToday()
        {
            return (from day in Items
                    where day.Date.Date == DateTime.Now.Date
                    select day).LastOrDefault();
        }

        public WorkDayGridItemViewModel GetToday()
        {
            var existing = FindToday();

            return existing ?? new WorkDayGridItemViewModel(new WorkDayData());
        }
    }
}