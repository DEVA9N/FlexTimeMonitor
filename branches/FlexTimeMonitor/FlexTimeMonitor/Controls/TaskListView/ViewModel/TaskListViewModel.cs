using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using A9N.FlexTimeMonitor.Data.Tasks;
using A9N.FlexTimeMonitor.Data.WorkTasks;

namespace A9N.FlexTimeMonitor.Controls
{
    public class TaskListViewModel
    {
        public TaskListViewModel(TaskCollection tasks)
        {
            this.Tasks = tasks;
        }

        public IEnumerable<Task> Tasks { get; private set; }
    }
}
