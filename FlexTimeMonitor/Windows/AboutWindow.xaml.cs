using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Constructor
        public AboutWindow()
        {
            InitializeComponent();

            var doc = new FlowDocument();

            doc.Blocks.Add(new Paragraph(new Run("About " + Properties.Resources.ApplicationName)));
            doc.Blocks.Add(new Paragraph(new Run(Properties.Resources.ApplicationName + GetApplicationVersion() + Environment.NewLine +  Environment.NewLine)));
            doc.Blocks.Add(new Paragraph(new Run("Copyright © 2009-2013 Andre Janßen" + Environment.NewLine + Environment.NewLine)));
            doc.Blocks.Add(new Paragraph(new Run("Visit http://a9n.de for further information")));


            this.richTextBox.IsReadOnly = true;
            this.richTextBox.Document.Blocks.Clear();
            this.richTextBox.Document = doc;

        }
        #endregion

        #region Private Methods
        private String GetApplicationVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                // This version matches the published version and is only available in the published application
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                // This return the AssemblyInfos' version which differs to the deployment version
                return System.Windows.Forms.Application.ProductVersion;
            }
            // Todo: Check if it a good idea to sync those numbers
        }
        #endregion
    }
}
