using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Work
{
    /// <summary>
    /// This class reads and writes objects to history files.
    /// Manager for generic xml-(de)serialization
    /// </summary>
    class WorkHistoryFile
    {
        private readonly String _fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkHistoryFile"/> class.
        /// </summary>
        public WorkHistoryFile()
        {
            _fileName = GetDefaultFileName();
        }

        /// <summary>
        /// Read object of type T from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>``0.</returns>
        private T Read<T>(String fileName)
        {
            if (!File.Exists(fileName))
            {
                return default(T);
            }

            using (var reader = new XmlTextReader(fileName))
            {
                var x = new XmlSerializer(typeof(T));
                return (T)x.Deserialize(reader);
            }
        }

        /// <summary>
        /// Write object to file. No additional type info necessary.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="output">The output.</param>
        private static void Write(String fileName, Object output)
        {
            using (var xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8) { Formatting = Formatting.Indented })
            {
                var serializer = new XmlSerializer(output.GetType());
                serializer.Serialize(xmlWriter, output);
            }
        }

        /// <summary>
        /// Get the default history file name for the current user.
        /// </summary>
        /// <returns>String.</returns>
        private static String GetDefaultFileName()
        {
            String myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            return Path.Combine(myDocuments, Resources.ApplicationName, Resources.FileName);
        }

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
                History.AddToday();
            }
            else
            {
                // No previous history so create a new one
                History = new WorkHistory();
                History.AddToday();
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
            history.Today.UpdateEnd();

            // Save the file
            Write(fileName, history);
        }

        /// <summary>
        /// Gets the history.
        /// </summary>
        /// <value>The history.</value>
        public WorkHistory History { get; private set; }
    }
}
