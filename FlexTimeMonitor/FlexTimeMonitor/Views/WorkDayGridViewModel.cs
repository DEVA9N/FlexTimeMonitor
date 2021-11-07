﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Work;

namespace A9N.FlexTimeMonitor.Views
{
    internal class WorkDayGridViewModel : ViewModel
    {
        public ObservableCollection<WorkDayGridItemViewModel> Items { get; }

        public WorkDayGridItemViewModel Today { get; }

        public WorkDayGridViewModel(IEnumerable<WorkDayEntity> items)
        {
            var viewModels = CreateViewModels(items);
            Items = new ObservableCollection<WorkDayGridItemViewModel>(viewModels);

            Today = GetToday();
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
            if (items.All(d => d.Date.Date != DateTime.Now.Date))
            {
                items.Add(new WorkDayEntity { Start = DateTime.Now });
            }

            return items;
        }

        private WorkDayGridItemViewModel GetToday()
        {
            return (from day in Items
                    where day.Date.Date == DateTime.Now.Date
                    select day).LastOrDefault();
        }
    }
}