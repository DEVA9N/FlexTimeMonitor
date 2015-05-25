using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
{
    interface IHistoryTreeItem
    {
        int Number { get; }

        String DisplayName { get; }

    }
}
