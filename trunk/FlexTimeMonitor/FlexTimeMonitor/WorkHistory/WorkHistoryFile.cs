using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor
{
    /// <summary>
    /// This class reads and writes objects to history files.
    /// Manager for generic xml-(de)serialization
    /// </summary>
    class WorkHistoryFile
    {
        private String _fileName;

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkHistoryFile"/> class.
        /// </summary>
        public WorkHistoryFile()
            : this(GetDefaultFileName())
        {

        }

        /// <summary>
        /// Create new instance with defined file name
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public WorkHistoryFile(String fileName)
        {
            this._fileName = fileName;
        }
        #endregion

        #region XML (de)serialization
        /// <summary>
        /// Read object of type T from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>``0.</returns>
        private T Read<T>(String fileName)
        {
            if (File.Exists(fileName))
            {
                using (XmlReader reader = XmlReader.Create(fileName))
                {
                    XmlSerializer x = new XmlSerializer(typeof(T));
                    return (T)x.Deserialize(reader);
                }
            }
            return default(T);
        }

        /// <summary>
        /// Write object to file. No additional type info necessary.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="output">The output.</param>
        private static void Write(String fileName, Object output)
        {
            StringBuilder builder = new StringBuilder();
            using (XmlWriter writer = XmlWriter.Create(builder))
            {
                XmlSerializer serializer = new XmlSerializer(output.GetType());
                serializer.Serialize(writer, output);

                XmlDocument outputDocument = new XmlDocument();
                outputDocument.LoadXml(builder.ToString());
                outputDocument.Save(fileName);
            }
        }
        #endregion

        #region Helper methods
        /// <summary>
        /// Get the default history file name for the current user.
        /// </summary>
        /// <returns>String.</returns>
        private static String GetDefaultFileName()
        {
            String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return Path.Combine(myDocuments, Resources.ApplicationName, Resources.FileName);
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
            if (File.Exists(_fileName))
            {
                // It is still possible that the data is invalid and an exception is thrown.
                // This is good though! The other classes should get an option to handle the error.
                History = Read<WorkHistory>(_fileName);
            }
            else
            {
                // No previous history so create a new one
                History = new WorkHistory();
            }

            // Add the current day as new item 
            if (History.Today == null)
            {
                History.Today = new WorkDay();
            }

            try
            {
                // This will make sure that the start is logged correctly even if the computer crashes
                this.Save();
            }
            catch (Exception)
            {
                throw new InvalidOperationException("Cannot access file for writing " + _fileName);
            }
        }


        /// <summary>
        /// Save history to file.
        /// Policy: Warn on load, solve on close.
        /// </summary>
        public void Save()
        {
            Save(this.History, _fileName);
        }

        private static void Save(WorkHistory history, String fileName)
        {
            FileInfo info = new FileInfo(fileName);

            // Make the file ready to be saved - this especially involves the path which must be present
            if (info.Exists == false)
            {
                // Make sure the parent directory exists
                if (info.Directory != null && info.Directory.Exists == false)
                {
                    info.Directory.Create();
                }
            }

            // Set the end of today
            history.Today.End = DateTime.Now.TimeOfDay;

            // Create a backup file by moving it. If the write fails the backup will 
            String backupName = CreateBackup(fileName);

            // Save the file
            Write(fileName, history);

            // After a successful write the backup will be removed
            RemoveBackup(backupName);
        }
        #endregion



        #region Backup

        /// <summary>
        /// Creates a backup of the current history file. The file is located in the FlexTimeMonitor's
        /// directory.
        /// </summary>
        /// <returns>String.</returns>
        /// <exception cref="System.InvalidOperationException">Unable to detect backup path.</exception>
        public String CreateBackup()
        {
            return CreateBackup(_fileName);
        }

        /// <summary>
        /// Creates a backup of the current history file. The file is located in the FlexTimeMonitor's
        /// directory.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>String.</returns>
        /// <exception cref="System.InvalidOperationException">Unable to detect backup path.</exception>
        private static String CreateBackup(String fileName)
        {
            if (File.Exists(fileName))
            {
                var backupFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss-") + Path.GetFileName(fileName);
                var backupPath = Path.GetDirectoryName(fileName);

                if (backupPath == null)
                {
                    throw new InvalidOperationException("Unable to detect backup path.");
                }

                var backupFullFileName = Path.Combine(backupPath, backupFileName);

                File.Move(fileName, backupFullFileName);

                return backupFullFileName;
            }

            return String.Empty;
        }

        /// <summary>
        /// Removes the backup.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static void RemoveBackup(String fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
        #endregion

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <value>The history.</value>
        public WorkHistory History { get; private set; }
    }
}
