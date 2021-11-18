using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Views
{
    internal sealed class WorkDayGridItemViewModel : ViewModel
    {
        private const String DefaultTimeFormat = "T";
        private readonly WorkDayEntity _entity;
        private readonly WorkDayProgress _progress;

        public WorkDayGridItemViewModel(WorkDayEntity entity)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _progress = new WorkDayProgress(entity, Settings.Default.WorkPeriod + Settings.Default.BreakPeriod);
        }

        public String DayOfWeek => _entity.Start.DayOfWeek.ToString();

        public String Date => _entity.Start.ToString("yyyy-MM-dd");

        public String Start
        {
            get => _entity.Start.ToString(DefaultTimeFormat);
            set => _entity.Start = _entity.Start.Date + DateTime.Parse(value).TimeOfDay;
        }

        public String End
        {
            get => _entity.End.ToString(DefaultTimeFormat);
            set => _entity.End = _entity.End.Date + DateTime.Parse(value).TimeOfDay;
        }

        /// <summary>
        /// Gets or sets the discrepancy. Discrepancy is a positive or negative time offset that is taken into account
        /// when calculating the total workday time. For example a skipped lunch break can be set by +1h or a doctor's
        /// appointment can be set by -1h.
        /// </summary>
        /// <value>The discrepancy.</value>
        public String Discrepancy
        {
            get => _entity.Discrepancy.ToString(DefaultTimeFormat);
            set => _entity.Discrepancy = DateTime.MinValue + DateTime.Parse(value).TimeOfDay;
        }

        public String Note
        {
            get => _entity.Note;
            set => _entity.Note = value;
        }

        public String OverTime => _progress.OverTime.ToHhmmss();

        public bool IsOddWeek
        {
            get
            {
                var calendar = DateTimeFormatInfo.CurrentInfo?.Calendar;
                var weekNumber = calendar?.GetWeekOfYear(_entity.Start, CalendarWeekRule.FirstDay, System.DayOfWeek.Monday) ?? 0;

                if (weekNumber > 0)
                {
                    return weekNumber % 2 > 0;
                }

                return true;
            }
        }

        public bool IsToday => _entity.Start.Date == DateTime.Now.Date;

        public bool HasNegativeOvertime => !IsToday && _progress.OverTime < TimeSpan.Zero;

        public WorkDayEntity ToEntity()
        {
            if (IsToday)
            {
                _entity.End = DateTime.Now;
            }

            return _entity;
        }
    }
}
