//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Windows.Controls;

//namespace A9N.FlexTimeMonitor.Controls.HistoryTree.TreeItems
//{
//    class DayTreeItem : TreeViewItem
//    {

//        public String DisplayName { get { return this.Day.Date.Day.ToString(); } }

//        public WorkDay Day { get; private set; }

//        public DayTreeItem(WorkDay day)
//        {
//            if (day == null)
//            {
//                throw new ArgumentNullException("day");
//            }

//            this.Day = day;
//        }

//        public override string ToString()
//        {
//            return this.DisplayName;
//        }
//    }
//}
