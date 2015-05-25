using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Class TimeSpanHelper
    /// </summary>
    static class TimeSpanHelper
    {
        /// <summary>
        /// Puts the TimeSpan in [-]hh:mm:ss format.
        /// hh is 00-Int32.Maximum
        /// mm is 00-59
        /// ss is 00-59
        /// This is a necessary workaround for the formating issues of TimeSpan.
        /// TimeSpan.ToString() supports negative values but will also show milliseconds
        /// TimeSpan.ToString("T") does not support negative values
        /// TimeSpan.ToString(@"hh:mm:ss") does not support negative values
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>String.</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static String ToHhmmss(TimeSpan t)
        {
            String sign = t < TimeSpan.Zero ? "-" : "";
            String hours = Math.Abs((int)t.TotalHours).ToString("00");
            String minutes = Math.Abs(t.Minutes).ToString("00");
            String seconds = Math.Abs(t.Seconds).ToString("00");
            return String.Format("{0}{1}:{2}:{3}", sign, hours, minutes, seconds);
        }
    }
}
