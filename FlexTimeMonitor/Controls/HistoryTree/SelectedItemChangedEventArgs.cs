using A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree
{
    /// <summary>
    /// Delegate SelectedItemChangedEventHandler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="SelectedItemChangedEventArgs"/> instance containing the event data.</param>
    internal delegate void SelectedItemChangedEventHandler(Object sender, SelectedItemChangedEventArgs e);

    /// <summary>
    /// Class SelectedItemChangedEventArgs.
    /// </summary>
    internal class SelectedItemChangedEventArgs
    {
        /// <summary>
        /// Gets the selected item.
        /// </summary>
        /// <value>The selected item.</value>
        public IHistoryTreeItem SelectedItem { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectedItemChangedEventArgs"/> class.
        /// </summary>
        /// <param name="selectedItem">The selected item.</param>
        public SelectedItemChangedEventArgs(IHistoryTreeItem selectedItem)
        {
            this.SelectedItem = selectedItem;
        }
    }
}
