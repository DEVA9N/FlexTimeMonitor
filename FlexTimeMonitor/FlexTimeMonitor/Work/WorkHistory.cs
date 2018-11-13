using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.Work
{
    public class WorkHistory : ObservableCollection<WorkDay>
    {
        public WorkDay GetToday()
        {
            return (from day in this
                    where day.Date.Date == DateTime.Now.Date
                    select day).LastOrDefault();
        }

        public void AddToday()
        {
            var existing = GetToday();

            if (existing != null)
            {
                return;
            }

            Add(new WorkDay());
        }
    }
}
