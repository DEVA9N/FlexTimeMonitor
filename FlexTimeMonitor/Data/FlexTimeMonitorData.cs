using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using A9N.FlexTimeMonitor.Properties;

namespace A9N.FlexTimeMonitor.Data
{
    class FlexTimeMonitorData
    {
        private readonly String historyFileName;
        private readonly String taskListFileName;

        public List<WorkDay> WorkDays { get; private set; }
        public List<Task> Tasks { get; private set; }

        public FlexTimeMonitorData(String dataPath)
        {
            if (String.IsNullOrEmpty(dataPath))
            {
                throw new ArgumentException(nameof(dataPath));
            }

            this.historyFileName = Path.Combine(dataPath, Settings.Default.HistoryFileName);
            this.taskListFileName = Path.Combine(dataPath, Settings.Default.TasksFileName);
        }

        public void Load()
        {
            this.WorkDays = LoadWorkDays(historyFileName);

            this.Tasks = LoadTasks(taskListFileName);
        }

        public void Save()
        {
            WriteFile(historyFileName, this.WorkDays);

            WriteFile(taskListFileName, this.Tasks);
        }

        private static List<WorkDay> LoadWorkDays(String fileName)
        {
            try
            {
                var result = ReadFile<List<WorkDay>>(fileName);

                var today = result.Find(day => day.Date.Date == DateTime.Now.Date);

                if (today == null)
                {
                    result.Add(new WorkDay());
                }

                return result;

            }
            catch (FileNotFoundException)
            {
                return new List<WorkDay> { new WorkDay() };
            }
        }

        private static List<Task> LoadTasks(String fileName)
        {
            try
            {
                var result = ReadFile<List<Task>>(fileName);

                return result;

            }
            catch (FileNotFoundException)
            {
                return new List<Task>();
            }
        }

        private static T ReadFile<T>(String fileName)
        {
            using (var reader = XmlReader.Create(fileName))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(reader);
            }
        }

        private static void WriteFile(String fileName, Object output)
        {
            var builder = new StringBuilder();
            using (var writer = XmlWriter.Create(builder))
            {
                var serializer = new XmlSerializer(output.GetType());
                serializer.Serialize(writer, output);

                var outputDocument = new XmlDocument();
                outputDocument.LoadXml(builder.ToString());
                outputDocument.Save(fileName);
            }
        }
    }
}
