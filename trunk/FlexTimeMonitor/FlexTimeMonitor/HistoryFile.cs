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
    class HistoryFile
    {
        private String fileName;

        /// <summary>
        /// Create new instance with defined file name
        /// </summary>
        /// <param name="fileName"></param>
        public HistoryFile(String fileName)
        {
            this.fileName = fileName;
        }

        /// <summary>
        /// Will provide a WorkHistory file from file. If there is problem while loading it will
        /// Policy: Warn on load, solve on close.
        /// </summary>
        /// <returns>valid history data</returns>
        /// <exception cref="System.IO.FileLoadException">Thrown when reading from invalid file.</exception>
        public WorkHistory Load()
        {
            if (File.Exists(fileName))
            {
                // It is still possible that the data is invalid and an exception is thrown.
                // This is good though! The User of this method should decide what to do with a broken file.
                try
                {
                    return Read<WorkHistory>(Settings.Default.LogfileName);
                }
                catch (Exception e)
                {
                    throw new System.IO.FileLoadException("Unable read data (" + fileName + "). Manually repair or erase file.");
                }
            }
            else
            {
                // There is an empty file - there is nothing to return.
                return new WorkHistory();
            }
        }


        /// <summary>
        /// Save history to file.
        /// Policy: Warn on load, solve on close.
        /// </summary>
        public void Save(WorkHistory history)
        {
            try
            {
                FileInfo info = new FileInfo(fileName);

                // Make the file ready to be saved - this espacially involves the path which must be present
                if (info.Exists == false)
                {
                    // Make sure the parent directory exists
                    if (info.Directory.Exists == false)
                    {
                        info.Directory.Create();
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to access history file" + fileName + "\n" + e);
            }

            try
            {
                Write(fileName, history);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Unable to save history to file " + fileName + "\n" + e);
            }
        }

        /// <summary>
        /// Creates a backup of the current history file. The file is located in the FlexTimeMonitor's
        /// directory. The name will include the date and time of the backup.
        /// </summary>
        public void CreateBackup()
        {
            throw new NotImplementedException();
        }


        #region XML (de)serialization

        /// <summary>
        /// Read object of type T from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
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
        /// Write object to file. No additional type info neccessary.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="output"></param>
        private void Write(String fileName, Object output)
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
    }
}
