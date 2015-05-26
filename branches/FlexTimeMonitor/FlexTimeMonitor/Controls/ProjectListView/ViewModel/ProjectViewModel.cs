using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Controls
{
    public class ProjectViewModel
    {
        public String Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
