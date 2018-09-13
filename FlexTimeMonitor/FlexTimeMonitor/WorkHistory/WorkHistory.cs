using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.WorkHistory
{
    public class WorkHistory : ObservableCollection<WorkDay>
    {
        private WorkDay _today;

        private WorkDay GetToday()
        {
            if (_today == null)
            {
                return (from day in this
                        where day.Date.Date == DateTime.Now.Date
                        select day).LastOrDefault();
            }

            return _today;
        }

        public WorkDay Today
        {
            get
            {
                if (_today == null)
                {
                    _today = GetToday();

                    // Well it might look like this is a good place to automatically create a new day if can't be found.
                    // But it is not! If the value is not null the programmer has no need to create a new instance and
                    // that means that the Start property will be set when this property is first accessed - which is
                    // likely when the End should be assigned - making Start and End happen at the same time.
                    // 
                    // Keeping this property null enforces the creation of a new WorkDay at the very start of the program,
                    // which automatically leads to proper data.
                }

                // Can still be null
                return _today;
            }
            set
            {
                if (value != null)
                {
                    var existing = GetToday();

                    if (existing != null)
                    {
                        throw new InvalidOperationException("An object from today already exists");
                    }

                    _today = value;

                    this.Add(_today);
                }
            }
        }
    }
}
