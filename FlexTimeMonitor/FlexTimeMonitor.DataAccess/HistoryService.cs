using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.DataAccess.Serialization;
using A9N.FlexTimeMonitor.Entities;

namespace A9N.FlexTimeMonitor.DataAccess
{
    public sealed class HistoryService : IWorkHistoryService
    {
        private readonly HistoryFile _historyFile;

        public HistoryService(String applicationName, String fileName)
        {
            _historyFile = new HistoryFile(Environment.SpecialFolder.ApplicationData, applicationName, fileName);
        }

        public IEnumerable<WorkDayData> GetHistory()
        {
            var history = _historyFile.Exists
                ? _historyFile.Load<IEnumerable<WorkDayData>>()
                : Enumerable.Empty<WorkDayData>();

            return history;
        }

        public void SaveHistory(IEnumerable<WorkDayData> days)
        {
            _historyFile.Save(days);
        }
    }
}
