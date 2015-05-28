using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Helper;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// Class WorkHistory is a serializable list of WorkDay objects.
    /// </summary>
    public class WorkHistory : List<WorkDay>
    {
        private WorkDay _today;

        #region Helper methods
        /// <summary>
        /// Get the default history file name for the current user.
        /// </summary>
        /// <returns>String.</returns>
        private static String GetDefaultFileName()
        {
            String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return System.IO.Path.Combine(myDocuments, Properties.Resources.ApplicationName, Settings.Default.HistoryFileName);
        }
        #endregion

        #region Load / Save

        /// <summary>
        /// Will provide a WorkHistory file from file. If there is problem while loading it will
        /// Policy: Warn on load, solve on close.
        /// </summary>
        /// <returns>valid history data</returns>
        public void Load()
        {
            var fileName = GetDefaultFileName();

            this.Load(fileName);
        }

        /// <summary>
        /// Loads the specified _file name.
        /// </summary>
        /// <param name="fileName">Name of the _file.</param>
        /// <exception cref="System.InvalidOperationException">
        /// corrupt history file
        /// or
        /// Cannot access file for writing  + fileName
        /// </exception>
        private void Load(String fileName)
        {
            if (File.Exists(fileName))
            {
                // It is still possible that the data is invalid and an exception is thrown.
                // This is good though! The other classes should get an option to handle the error.
                var temp = XmlHelper.Read<WorkHistory>(fileName);

                if (temp == null)
                {
                    throw new InvalidOperationException(String.Format("Corrupt history file '{0}'.", fileName));
                }

                foreach (var item in temp)
                {
                    this.Add(item);
                }
            }

            // Add the current day as new item 
            if (this.Today == null)
            {
                this.Today = new WorkDay();
            }

            try
            {
                // This will make sure that the start is logged correctly even if the computer crashes
                this.Save();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Cannot access file for writing " + fileName);
            }
        }


        /// <summary>
        /// Save history to file.
        /// Policy: Warn on load, solve on close.
        /// </summary>
        /// <param name="history">The history.</param>
        public void Save()
        {
            var fileName = GetDefaultFileName();

            this.Save(fileName);
        }

        /// <summary>
        /// Saves the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private void Save(String fileName)
        {
            FileInfo info = new FileInfo(fileName);

            // Make the file ready to be saved - this especially involves the path which must be present
            if (info.Exists == false)
            {
                // Make sure the parent directory exists
                if (info.Directory.Exists == false)
                {
                    info.Directory.Create();
                }
            }

            //var parent = Directory.GetParent(fileName).FullName;
            //var tempName = Path.GetTempFileName();
            //var tempFullFileName = Path.Combine(parent, tempName);

            // Set the end of today
            this.Today.End = DateTime.Now;

            // Save the file to a temporary file first
            // This is supposed to make the saving more secure
            // because it will not replace the original file until everything
            // has been written to disk
            XmlHelper.Write(fileName, this);

            // The Move is supposed to take no time and reduce the risk of 
            // damaged files when shutting down the computer.
            // If there is a problem it is expected, that at least 2 files exist
            //File.Move(tempFullFileName, fileName);
        }
        #endregion

        /// <summary>
        /// Gets today.
        /// </summary>
        private WorkDay GetToday()
        {
            if (_today == null)
            {
                var allTodays = (from day in this
                                where day.Date.Date == DateTime.Now.Date
                                select day).ToList();

                if (allTodays.Any())
                {
                    return allTodays.Last();
                }
            }
            return _today;
        }

        /// <summary>
        /// Gets the today.
        /// </summary>
        /// <value>The today.</value>
        public WorkDay Today
        {
            get
            {
                if (_today == null)
                {
                    _today = GetToday();

                    // Well it might look like this is a good place to automatically create a new day if can't be found.
                    // But it is not! If the value is not null the programmer has no need to create a new instance and
                    // that means that the Start property will be set when this property is first accessed - which is
                    // likely when the End should be assigned - making Start and End happen at the same time.
                    // 
                    // Keeping this property null enforces the creation of a new WorkDay at the very start of the program,
                    // which automatically leads to proper data.
                }

                // Can still be null
                return _today;
            }
            set
            {
                if (value != null)
                {
                    var existing = GetToday();

                    if (existing != null)
                    {
                        throw new InvalidOperationException("An object from today already exists");
                    }

                    _today = value;

                    this.Add(_today);
                }
            }
        }
    }
}
