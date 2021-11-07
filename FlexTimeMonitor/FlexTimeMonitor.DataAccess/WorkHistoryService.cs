using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using A9N.FlexTimeMonitor.Contracts;
using A9N.FlexTimeMonitor.DataAccess.FileAccess;

namespace A9N.FlexTimeMonitor.DataAccess
{
    public sealed class WorkHistoryService : IWorkHistoryService
    {
        private readonly HistoryFile _historyFile;

        public WorkHistoryService(String applicationName)
        {
            _historyFile = new HistoryFile(Environment.SpecialFolder.ApplicationData, applicationName);
        }

        public IEnumerable<WorkDayEntity> GetItems()
        {
            var history = _historyFile.Exists
                ? _historyFile.Load<IEnumerable<WorkDayEntity>>()
                : Enumerable.Empty<WorkDayEntity>();

            return history;
        }

        public void SaveItems(IEnumerable<WorkDayEntity> days)
        {
            _historyFile.Save(days);
        }
    }
}
