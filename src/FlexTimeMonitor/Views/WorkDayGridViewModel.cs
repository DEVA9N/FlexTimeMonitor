﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Mvvm;

namespace A9N.FlexTimeMonitor.Views
{
    internal class WorkDayGridViewModel : ViewModel
    {
        public ObservableCollection<WorkDayListItemViewModel> Items { get; }

        public WorkDayListItemViewModel Today { get; }

        public SelectionViewModel Selection { get; set; }
        
        public bool SelectionPopupVisible { get; set; }

        public WorkDayGridViewModel(IEnumerable<WorkDayEntity> items)
        {
            Items = CreateViewModels(items);

            Today = Items.FirstOrDefault(i => i.IsToday);
        }

        private ObservableCollection<WorkDayListItemViewModel> CreateViewModels(IEnumerable<WorkDayEntity> items)
        {
            var viewModels = from item in items
                             select new WorkDayListItemViewModel(item);

            return new ObservableCollection<WorkDayListItemViewModel>(viewModels);
        }

    }
}