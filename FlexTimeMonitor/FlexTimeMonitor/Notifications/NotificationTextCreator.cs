using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.Extensions;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Views
{
    internal sealed class NotificationTextCreator : ViewModel
    {
        public static String TryCreateText(WorkDayEntity entity)
        {
            if (entity == null)
            {
                return String.Empty;
            }

            return CreateText(entity);
        }

        public static String CreateText(WorkDayEntity entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var times = new WorkDayProgress(entity, Settings.Default.WorkPeriod + Settings.Default.BreakPeriod);

            return CreateText(entity, times);
        }

        private static String CreateText(WorkDayEntity entity, WorkDayProgress progress)
        {
            var text = $"{"Start:",-16}\t{entity.Start.TimeOfDay.ToHhmmss(),10}\n";
            text += $"{"Estimated:",-16}\t{progress.Estimated.ToHhmmss(),10}\n";
            text += $"{"Elapsed:",-16}\t{progress.Elapsed.ToHhmmss(),10}\n";
            text += $"{"Remaining:",-16}\t{progress.Remaining.ToHhmmss(),10}\n";

            return text;
        }
    }
}
