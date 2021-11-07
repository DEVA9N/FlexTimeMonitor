using System.Collections.Generic;
using A9N.FlexTimeMonitor.Entities;

namespace A9N.FlexTimeMonitor.Contracts
{
    public interface IWorkHistoryService
    {
        IEnumerable<WorkDayData>  GetHistory();

        void SaveHistory(IEnumerable<WorkDayData> days);
    }
}
