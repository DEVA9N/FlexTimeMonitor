using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.DataAccess.Legacy;
using Newtonsoft.Json;

namespace A9N.FlexTimeMonitor.DataAccess
{
    public sealed class WorkHistoryService : IWorkHistoryService
    {
        private readonly String _path;
        private readonly String _fileName;
        private readonly XmlFileImporter _importer;

        public WorkHistoryService(String applicationName)
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ArgumentException($"'{nameof(applicationName)}' cannot be null or empty.", nameof(applicationName));
            }

            _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), applicationName);
            _fileName = Path.Combine(_path, "History.json");

            _importer = new XmlFileImporter(applicationName);
        }

        public IEnumerable<WorkDayEntity> GetItems()
        {
            if (_importer.CanImport)
            {
                return _importer.Import();
            }

            var json = File.Exists(_fileName) ? File.ReadAllText(_fileName) : String.Empty;
            var result = JsonConvert.DeserializeObject<IEnumerable<WorkDayEntity>>(json);

            return result ?? Enumerable.Empty<WorkDayEntity>();
        }

        public void SaveItems(IEnumerable<WorkDayEntity> days)
        {
            var json = JsonConvert.SerializeObject(days, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            // Creates all directories if required
            Directory.CreateDirectory(_path);
            File.WriteAllText(_fileName, json); ;

            if (_importer.CanImport)
            {
                _importer.Cleanup();
            }
        }
    }
}
