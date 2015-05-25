using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace TimeManagerDlx
{
    public partial class MainWindow : Form
    {
        private String historyFileName;
        private WorkDay today;
        private WorkHistory history;

        // BalloonTip fix - without it will prevent from registering the double click
        bool listenToMouseMove = true;

        public MainWindow()
        {
            InitializeComponent();

            // Add version string
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                String version = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
                Text += " - " + version;
            }

            // Generate history filename
            String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\TimeManagerDlx";
            String file = "\\timeLog.xml";
            historyFileName = path + file;

            if (Directory.Exists(path) == false)
            {
                // Create directory
                Directory.CreateDirectory(path);
            }

            // Read history
            ReadHistory();

            // Get today from history - never get it somewhere else!
            today = history.Today;

            // Display history in listview
            DisplayHistory();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            notifyIcon1.Visible = false;

            // Set end time and save object
            if (today != null)

                today.End = DateTime.Now;

            // Write history
            WriteHistory();
        }

        #region File handling

        private void ReadHistory()
        {
            if (File.Exists(historyFileName))
            {
                XmlReader reader = XmlReader.Create(historyFileName);
                XmlSerializer x = new XmlSerializer(typeof(WorkHistory));
                history = (WorkHistory)x.Deserialize(reader);
                reader.Close();
            }

            if (history == null)
            {
                history = new WorkHistory();
            }
        }

        private void WriteHistory()
        {
            StringBuilder output = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(output);

            XmlSerializer serializer = new XmlSerializer(history.GetType());
            serializer.Serialize(writer, history);

            XmlDocument outputDocument = new XmlDocument();
            outputDocument.LoadXml(output.ToString());
            outputDocument.Save(historyFileName);
        }

        #endregion

        /// <summary>
        /// Display history in listview
        /// </summary>
        private void DisplayHistory()
        {
            this.listView1.Items.Clear();

            foreach (WorkDay w in history)
            {
                ListViewItem l = new ListViewItem(w.Start.ToString("d"));
                l.SubItems.Add(w.Start.ToString("t"));
                l.SubItems.Add(w.End.ToString("t"));
                l.SubItems.Add(w.Estimated.ToString("t"));
                l.SubItems.Add(TimeSpanToString(w.Difference));
                l.SubItems.Add(TimeSpanToString(w.Difference - (Properties.Settings.Default.WorkPeriod + Properties.Settings.Default.BreakPeriod)));

                this.listView1.Items.Add(l);
            }
        }

        /// <summary>
        /// Display selection in status bar
        /// </summary>
        /// <param name="dayCount"></param>
        /// <param name="timeOverall"></param>
        /// <param name="timeIntended"></param>
        /// <param name="timeDifference"></param>
        private void DisplaySelection(String dayCount, String timeOverall, String timeIntended, String timeDifference)
        {
            statusStrip1.Items.Clear();

            statusStrip1.Items.Add(new ToolStripStatusLabel("Days: "));
            statusStrip1.Items.Add(new ToolStripStatusLabel(dayCount));
            statusStrip1.Items.Add(new ToolStripSeparator());

            statusStrip1.Items.Add(new ToolStripStatusLabel("Overall: "));
            statusStrip1.Items.Add(new ToolStripStatusLabel(timeOverall));
            statusStrip1.Items.Add(new ToolStripSeparator());

            statusStrip1.Items.Add(new ToolStripStatusLabel("Intended: "));
            statusStrip1.Items.Add(new ToolStripStatusLabel(timeIntended));
            statusStrip1.Items.Add(new ToolStripSeparator());

            statusStrip1.Items.Add(new ToolStripStatusLabel("Difference: "));
            statusStrip1.Items.Add(new ToolStripStatusLabel(timeDifference));
        }

        /// <summary>
        /// TimeSpan to string formated: hh:mm
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private String TimeSpanToString(TimeSpan t)
        {
            if (t != null)
            {
                int hours = t.Hours < 0 ? t.Hours * -1 : t.Hours;
                int minutes = t.Minutes < 0 ? t.Minutes * -1 : t.Minutes;
                String sign = t.Hours < 0 || t.Minutes < 0 ? "-" : "";

                return sign + hours.ToString() + ":" + minutes.ToString("00");
            }
            return "";
        }

        private String TimeSpanTotalToString(TimeSpan t)
        {
            if (t != null)
            {
                int hours = (int)t.TotalMinutes / 60;
                int minutes = (int)t.TotalMinutes - (hours * 60);

                minutes = minutes < 0 ? minutes * -1 : minutes;

                return hours.ToString() + ":" + minutes.ToString("00");
            }
            return "";
        }

        #region Form Events

        /// <summary>
        /// Minimize to tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
            {
                Hide();
            }
        }

        /// <summary>
        /// Display from tray
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        /// <summary>
        /// Display status
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon1_MouseMove(object sender, MouseEventArgs e)
        {
            if (listenToMouseMove)
            {
                listenToMouseMove = false;
                balloonTipTimer.Interval = 5000;
                balloonTipTimer.Start();

                notifyIcon1.BalloonTipText = "Start: " + today.Start.ToString("t");
                notifyIcon1.BalloonTipText += "\nEstimated: " + today.Estimated.ToString("t");
                notifyIcon1.BalloonTipText += "\nElapsed: " + TimeSpanToString(today.Elapsed);
                notifyIcon1.BalloonTipText += "\nRemaining: " + TimeSpanToString(today.Remaining);

                notifyIcon1.ShowBalloonTip(10);
            }
        }

        /// <summary>
        /// Display selection results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            TimeSpan timeOverall = TimeSpan.Zero;
            TimeSpan timeIntended = TimeSpan.Zero;

            foreach (int i in listView1.SelectedIndices)
            {
                timeOverall += history[i].Difference - Properties.Settings.Default.BreakPeriod;
                timeIntended += Properties.Settings.Default.WorkPeriod;
            }

            String dayCount = listView1.SelectedItems.Count.ToString();
            String timeOverallStr = TimeSpanTotalToString(timeOverall);
            String timeIntendedStr = TimeSpanTotalToString(timeIntended);
            String timeDifferenceStr = TimeSpanToString(timeOverall - timeIntended);

            DisplaySelection(dayCount, timeOverallStr, timeIntendedStr, timeDifferenceStr);
        }

        /// <summary>
        /// If countdown finished enable balloon tip 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            listenToMouseMove = true;
        }


        #endregion

 

    }
}
