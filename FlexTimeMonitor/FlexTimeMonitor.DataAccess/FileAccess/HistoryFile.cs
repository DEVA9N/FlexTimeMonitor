using System;
using System.IO;
using A9N.FlexTimeMonitor.DataAccess.Serialization;

namespace A9N.FlexTimeMonitor.DataAccess
{
    internal sealed class HistoryFile
    {
        private readonly string _fileName;

        public bool Exists => File.Exists(_fileName);

        public HistoryFile(Environment.SpecialFolder folder, string applicationName, string fileName)
        {
            _fileName = CreateFileName(folder, applicationName, fileName);
        }

        private static string CreateFileName(Environment.SpecialFolder specialFolder, string applicationName, string fileName)
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