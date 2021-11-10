using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Mvvm;

namespace A9N.FlexTimeMonitor.Views
{
    internal class WorkDayGridViewModel : ViewModel
    {
        public ObservableCollection<WorkDayGridItemViewModel> Items { get; }

        public WorkDayGridItemViewModel Today { get; }

        public SelectionViewModel Selection { get; set; }
        
        public bool SelectionPopupVisible { get; set; }

        public WorkDayGridViewModel(IEnumerable<WorkDayEntity> items)
        {
            var viewModels = CreateViewModels(items);
            Items = new ObservableCollection<WorkDayGridItemViewModel>(viewModels);

            Today = Items.FirstOrDefault(i => i.IsToday);
        }

        private ObservableCollection<WorkDayGridItemViewModel> CreateViewModels(IEnumerable<WorkDayEntity> items)
        {
            var updatedItems = AddToday(items.ToList());
            var viewModels = from item in updatedItems
                             select new WorkDayGridItemViewModel(item);

            return new ObservableCollection<WorkDayGridItemViewModel>(viewModels);
        }

        private IEnumerable<WorkDayEntity> AddToday(List<WorkDayEntity> items)
        {
            if (items.All(d => d.Start.Date != DateTime.Now.Date))
            {
                items.Add(new WorkDayEntity { Start = DateTime.Now, End = DateTime.Now });
            }

            return items;
        }
    }
}