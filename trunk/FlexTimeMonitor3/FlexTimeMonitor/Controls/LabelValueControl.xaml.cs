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
    /// Interaction logic for LabelValueControl.xaml
    /// </summary>
    public partial class LabelValueControl : UserControl
    {
        public event TextChangedEventHandler ValueChanged;

        public String LabelText
        {
            get { return HeaderLabel.Content != null ? HeaderLabel.Content.ToString() : String.Empty; }
            set { HeaderLabel.Content = value; }
        }

        public String ValieText
        {
            get { return ValueTextBox.Text; }
            set { ValueTextBox.Text = value; }
        }

        public bool IsReadOnly
        {
            get { return this.ValueTextBox.IsReadOnly; }
            set { this.ValueTextBox.IsReadOnly = value; }
        }

        public LabelValueControl()
        {
            InitializeComponent();

            this.ValueTextBox.TextChanged += ValueTextBox_TextChanged;
        }

        void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, e);
            }
        }
    }
}
