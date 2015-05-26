using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace A9N.FlexTimeMonitor.Controls.HistoryTree
{
    /// <summary>
    /// Interaction logic for HistoryTreeControl.xaml
    /// </summary>
    public partial class HistoryTreeControl : UserControl
    {
        public event RoutedPropertyChangedEventHandler<object> SelectedItemChanged
        {
            add { historyTree.SelectedItemChanged += value; }
            remove { historyTree.SelectedItemChanged -= value; }
        }

        public HistoryTreeControl()
        {
            InitializeComponent();
        }

        //protected override void OnContentChanged(object oldContent, object newContent)
        //{
        //    base.OnContentChanged(oldContent, newContent);

        //    this.historyTree.SelectedIte
        //}
    }
}
