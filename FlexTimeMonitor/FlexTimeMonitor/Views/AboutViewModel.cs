using System;
using System.Reflection;
using A9N.FlexTimeMonitor.Mvvm;

namespace A9N.FlexTimeMonitor.Views
{
    internal sealed class AboutViewModel : ViewModel
    {
        public String Title { get; }
        public String Version { get; }
        public String Copyright { get; }
        public String Description { get; }

        public AboutViewModel()
        {
            Title = GetAssemblyAttribute<AssemblyTitleAttribute>(a => a.Title);
            Version = GetAssemblyVersion();
            Copyright = GetAssemblyAttribute<AssemblyCopyrightAttribute>(a => a.Copyright);
            Description = GetAssemblyAttribute<AssemblyDescriptionAttribute>(a => a.Description);
        }

        private String GetAssemblyVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;

            return version.ToString();
        }

        private static string GetAssemblyAttribute<T>(Func<T, string> value)  where T : Attribute
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var attribute = (T)Attribute.GetCustomAttribute(executingAssembly, typeof(T));

            return value.Invoke(attribute);
        }
    }
}
