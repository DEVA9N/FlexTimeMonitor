using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.Extensions
{
    internal static class TimeSpanExtension
    {
        /// <summary>
        /// Puts the TimeSpan in [-]h:mm:ss format.
        /// This is a necessary workaround for the formatting issues of TimeSpan.
        /// TimeSpan.ToString() supports negative values but will also show milliseconds
        /// TimeSpan.ToString("T") does not support negative values
        /// TimeSpan.ToString(@"hh:mm:ss") does not support negative values
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>String.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static String ToHhmmss(this TimeSpan t)
        {
            // Instead of manually building the string we simply drop the milliseconds so that they don't appear on screen
            var noMilliseconds = new TimeSpan(t.Days, t.Hours, t.Minutes, t.Seconds);

            return $"{noMilliseconds:c}";
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static String ToTotalHhmmss(this TimeSpan t)
        {
            if (t.Ticks < 0)
            {
                // Create positive representation
                t = t.Duration();

                return $"-{Math.Floor(t.TotalHours):N0}:{t.Minutes}:{t.Seconds}";
            }

            return $"{Math.Floor(t.TotalHours):N0}:{Math.Abs(t.Minutes)}:{Math.Abs(t.Seconds)}";
        }

        public static DateTime ToDateTime(this TimeSpan t, DateTime date)
        {
            // TimeSpan does not keep track of the date - that's why 'date' is required
            return new DateTime(date.Year, date.Month, date.Day, t.Hours, t.Minutes, t.Seconds);
        }

    }
}
