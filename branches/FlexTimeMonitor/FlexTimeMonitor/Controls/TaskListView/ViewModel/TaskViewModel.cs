using System;
using A9N.FlexTimeMonitor.Data;

namespace A9N.FlexTimeMonitor.Controls
{
    public class TaskViewModel
    {
        public TaskViewModel(Task task)
        {
            this.Data = task;
        }

        public Task Data { get; private set; }

        public Project Project
        {
            get { return Data.Project; }
            set { Data.Project = value; }
        }

        public DateTime Date
        {
            get { return Data.Date; }
            set { Data.Date = value; }
        }


        public DateTime Start
        {
            get { return Data.Start; }
            set { Data.Start = value; }
        }

        public DateTime End
        {
            get { return Data.End; }
            set { Data.End = value; }
        }


    }
}
