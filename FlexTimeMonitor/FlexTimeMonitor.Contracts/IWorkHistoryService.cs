using System.Collections.Generic;

namespace A9N.FlexTimeMonitor.Contracts
{
    public interface IWorkHistoryService
    {
        IEnumerable<WorkDayData>  GetHistory();

        void SaveHistory(IEnumerable<WorkDayData> days);
    }
}
