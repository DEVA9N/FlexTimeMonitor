using System;
using System.IO;
using A9N.FlexTimeMonitor.DataAccess.Serialization;

namespace A9N.FlexTimeMonitor.DataAccess.FileAccess
{
    internal sealed class HistoryFile
    {
        private const String DefaultFileName = "History.xml";
        private readonly String _fileName;

        public bool Exists => File.Exists(_fileName);

        public HistoryFile(Environment.SpecialFolder folder, String applicationName)
        {
            _fileName = CreateFileName(folder, applicationName, DefaultFileName);
        }

        private static String CreateFileName(Environment.SpecialFolder specialFolder, String applicationName, String fileName)
        {
            var roaming = Environment.GetFolderPath(specialFolder);

            return Path.Combine(roaming, applicationName, fileName);
        }

        private static void TouchDirectory(String fileName)
        {
            var info = new FileInfo(fileName);

            if (!info.Exists)
            {
                CreatePath(info.Directory);
            }
        }

        private static void CreatePath(DirectoryInfo directory)
        {
            if (directory != null && !directory.Exists)
            {
                directory.Create();
            }
        }

        public T Load<T>()
        {
            var result = XmlFileSerializer.Read<T>(_fileName);

            return result;
        }

        public void Save(object history)
        {
            TouchDirectory(_fileName);

            XmlFileSerializer.Write(_fileName, history);
        }
    }
}