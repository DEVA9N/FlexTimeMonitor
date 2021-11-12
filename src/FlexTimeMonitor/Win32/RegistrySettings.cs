using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.Versioning;

namespace A9N.FlexTimeMonitor.Win32
{
    /// <summary>
    /// Class RegistrySettings handles access to all FlexTimeMonitor settings that are stored in the registry.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class RegistrySettings
    {
        private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const string AutoStartKey = @"FlexTimeMonitor";
        private const string AutoStartTarget = @"AppData\Roaming\Microsoft\Windows\Start Menu\Programs\A9N\FlexTimeMonitor.appref-ms";

        private readonly string autoStartValue;

        public RegistrySettings()
        {
            // The application path changes for every installation. The only way to start it from the current
            // location is to start it via the start menu entry.
            var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            autoStartValue = Path.Combine(userProfilePath, AutoStartTarget);
        }

        /// <summary>
        /// Gets or sets a value indicating whether FlexTimeMonitor is automatically started when the user logs in.
        /// </summary>
        /// <value><c>true</c> if FlexTimeMonitor is automatically started; otherwise, <c>false</c>.</value>
        public bool AutoStart
        {
            get
            {

                var subKey = Registry.CurrentUser.OpenSubKey(RunKey);

                if (subKey != null)
                {
                    var result = subKey.GetValue(AutoStartKey, string.Empty);

                    return autoStartValue.Equals(result);
                }

                return false;
            }
            set
            {
                var subKey = Registry.CurrentUser.OpenSubKey(RunKey, true);

                if (subKey == null)
                {
                    // Something is wrong with the registry
                    return;
                }

                if (value)
                {
                    subKey.SetValue(AutoStartKey, autoStartValue);
                }
                else
                {
                    subKey.DeleteValue(AutoStartKey);
                }
            }
        }
    }
}
