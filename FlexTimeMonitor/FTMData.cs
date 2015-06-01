using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using A9N.FlexTimeMonitor.Data;
using A9N.FlexTimeMonitor.Data;
using A9N.FlexTimeMonitor.Helper;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor
{
    class FTMData
    {
        private String historyFileName;
        private String taskListFileName;

        public List<WorkDay> WorkDays { get; private set; }
        public List<Task> Tasks { get; private set; }

        public FTMData(String dataPath)
        {
            if (String.IsNullOrEmpty(dataPath))
            {
                throw new ArgumentException("dataPath");
            }

            this.historyFileName = Path.Combine(dataPath, Settings.Default.HistoryFileName);
            this.taskListFileName = Path.Combine(dataPath, Settings.Default.TasksFileName);
        }

        /// <summary>
        /// Loads this instance.
        /// </summary>
        public void Load()
        {
            this.WorkDays = LoadWorkDays(historyFileName);

            this.Tasks = LoadTasks(taskListFileName);
        }

        /// <summary>
        /// Saves this instance.
        /// </summary>
        public void Save()
        {
           DataSerializer.Write(historyFileName, this.WorkDays);

            DataSerializer.Write(taskListFileName, this.Tasks);
        }

        private static List<WorkDay> LoadWorkDays(String fileName)
        {
            try
            {
                var result = DataSerializer.Read<List<WorkDay>>(fileName);

                var today = result.Find(day => day.Date.Date == DateTime.Now.Date);

                if (today == null)
                {
                    result.Add(new WorkDay());
                }

                return result;

            }
            catch (FileNotFoundException)
            {
                var result = new List<WorkDay>();

                result.Add(new WorkDay());

                return result;
            }
        }

        private static List<Task> LoadTasks(String fileName)
        {
            try
            {
                var result = DataSerializer.Read<List<Task>>(fileName);

                return result;

            }
            catch (FileNotFoundException)
            {
                var result = new List<Task>();

                return result;
            }
        }
    }
}
