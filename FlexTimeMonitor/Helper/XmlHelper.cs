using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace A9N.FlexTimeMonitor.Helper
{
    /// <summary>
    /// Class XmlHelper.
    /// </summary>
    internal static class XmlHelper
    {
        #region XML (de)serialization
        /// <summary>
        /// Read object of type T from file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>``0.</returns>
        internal static T Read<T>(String fileName)
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
        internal static void Write(String fileName, Object output)
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
