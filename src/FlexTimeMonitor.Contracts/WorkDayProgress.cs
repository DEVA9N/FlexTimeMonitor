using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace A9N.FlexTimeMonitor.Contracts
{
    public sealed class WorkDayProgress
    {
        private readonly WorkDayEntity _entity;
        private readonly TimeSpan _requiredTotal;

        public WorkDayProgress(WorkDayEntity entity, TimeSpan requiredTotal)
        {
            _entity = entity ?? throw new ArgumentNullException(nameof(entity));
            _requiredTotal = requiredTotal;
        }

        private bool IsToday => _entity.Start.Date == DateTime.Now.Date;
        private DateTime End => IsToday ? DateTime.Now : _entity.End;

        public TimeSpan OverTime => Elapsed - _requiredTotal;
        public TimeSpan Elapsed => End.TimeOfDay - _entity.Start.TimeOfDay + _entity.Discrepancy.TimeOfDay;
        public TimeSpan Estimated => _entity.Start.TimeOfDay - _entity.Discrepancy.TimeOfDay + _requiredTotal;
        public TimeSpan Remaining => -OverTime;
    }
}
