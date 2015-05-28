using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using A9N.FlexTimeMonitor.Data.Tasks;

namespace A9N.FlexTimeMonitor.Data.WorkTasks
{
    public class TaskList : List<Task>
    {
        public TaskList()
        {
            this.Add(new Task() { Project = new Project() { Description = "Test project", Employer = "A9N", Name = "Test" } });

        }

    }
}
