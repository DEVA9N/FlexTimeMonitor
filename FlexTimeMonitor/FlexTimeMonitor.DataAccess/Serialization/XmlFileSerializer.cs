using System;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace A9N.FlexTimeMonitor.DataAccess.Serialization
{
    internal static class XmlFileSerializer
    {
        public static T Read<T>(String fileName)
        {
            using (var reader = new XmlTextReader(fileName))
            {
                var x = new XmlSerializer(typeof(T));
                return (T)x.Deserialize(reader);
            }
        }

        public static void Write(String fileName, Object output)
        {
            using (var xmlWriter = new XmlTextWriter(fileName, Encoding.UTF8) { Formatting = Formatting.Indented })
            {
                var serializer = new XmlSerializer(output.GetType());
                serializer.Serialize(xmlWriter, output);
            }
        }

    }
}
