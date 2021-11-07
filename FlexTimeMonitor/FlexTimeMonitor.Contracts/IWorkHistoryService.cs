using System.Collections.Generic;

namespace A9N.FlexTimeMonitor.Contracts
{
    public interface IWorkHistoryService
    {
        IEnumerable<WorkDayEntity>  GetItems();

        void SaveItems(IEnumerable<WorkDayEntity> days);
    }
}
