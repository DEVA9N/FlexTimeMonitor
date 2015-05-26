using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace A9N.FlexTimeMonitor.Controls
{
    class WorkDayGrid : DataGrid
    {
        public WorkDayGrid()
        {
            this.AllowDrop = true;

        }

        protected override void OnDragEnter(System.Windows.DragEventArgs e)
        {
            base.OnDragEnter(e);

            //if (!e.Data.GetDataPresent(ProjectList.DropType))
            //{
            //    e.Effects = DragDropEffects.None;
            //}
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var element = (UIElement)e.Source;

            int c = Grid.GetColumn(element);
            int r = Grid.GetRow(element);

            if (r != 0)
            {

            }
        }

        protected override void OnDrop(System.Windows.DragEventArgs e)
        {
            base.OnDrop(e);

            //if (e.Data.GetDataPresent(ProjectList.DropType))
            //{
            //    var pos = Mouse.GetPosition(this);

            //    var element = (UIElement)e.Source;

            //    int c = Grid.GetColumn(element);
            //    int r = Grid.GetRow(element);

            //    if (r != 0)
            //    {

            //    }

            //}
        }

    }
}
