using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Controls
{
    interface IDetailView
    {
        IEnumerable<WorkDay> History { get; }
    }
}
