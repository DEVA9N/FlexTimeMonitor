using A9N.FlexTimeMonitor.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace A9N.FlexTimeMonitor.DataAccess.Legacy
{
    internal sealed class XmlFileImporter
    {
        private const string DefaultFileName = "History.xml";
        private readonly string _fileName;

        public bool CanImport => File.Exists(_fileName);

        public XmlFileImporter(string applicationName)
        {
            _fileName = CreateFileName(Environment.SpecialFolder.MyDocuments, applicationName, DefaultFileName);
        }

        private static string CreateFileName(Environment.SpecialFolder specialFolder, string applicationName, string fileName)
        {
            var roaming = Environment.GetFolderPath(specialFolder);

            return Path.Combine(roaming, applicationName, fileName);
        }

        public void Cleanup()
        {
            if (File.Exists(_fileName))
            {
                File.Delete(_fileName);
            }
        }

        public IEnumerable<WorkDayEntity> Import()
        {
            var imported = ImportXml();

            var mapped = MapToEntities(imported);

            return mapped;
        }

        private List<WorkDay> ImportXml()
        {
            using (var reader = new XmlTextReader(_fileName))
            {
                var x = new XmlSerializer(typeof(List<WorkDay>));

                return (List<WorkDay>)x.Deserialize(reader);
            }
        }

        private IEnumerable<WorkDayEntity> MapToEntities(List<WorkDay> imported)
        {
            return imported
                .Where(x => x?.Data != null)
                .Select(x =>
                new WorkDayEntity
                {
                    Start = x.Data.Start,
                    End = x.Data.End,
                    Discrepancy = x.Data.Discrepancy,
                    Note = x.Data.Note
                });
        }
    }
}