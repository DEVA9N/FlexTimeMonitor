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

namespace A9N.FlexTimeMonitor.Controls
{
    /// <summary>
    /// Interaction logic for ProjectList.xaml
    /// </summary>
    public partial class ProjectList : UserControl
    {
        internal const String DropType = "Protocol";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectList"/> class.
        /// </summary>
        public ProjectList()
        {
            InitializeComponent();


            this.listView.PreviewMouseDown += listView_PreviewMouseDown;

#if DEBUG
            this.listView.ItemsSource = new Project[] {
            new Project() { Name = "Mein Project"},
            new Project() { Name = "Ein Project"},
            new Project() { Name = "Anderes Project"},
            };
#endif
        }

        void listView_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (this.listView.SelectedItem != null)
            {
                DataObject dragData = new DataObject(DropType, this.listView.SelectedItem);
                DragDrop.DoDragDrop(this, dragData, DragDropEffects.Move);
            }
        }

    }
}
