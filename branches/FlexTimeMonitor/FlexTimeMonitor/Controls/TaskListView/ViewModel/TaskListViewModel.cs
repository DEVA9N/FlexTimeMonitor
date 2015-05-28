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
        public TaskListViewModel(IEnumerable<Task> tasks)
        {
            this.Tasks = (from task in tasks 
                          select new TaskViewModel(task)).ToList();
        }

        public IList<TaskViewModel> Tasks { get; private set; }
    }
}
