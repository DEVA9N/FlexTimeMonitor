using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

namespace A9N.FlexTimeMonitor.Helper
{
    /// <summary>
    /// Class RegistrySettings handles access to all FlexTimeMonitor settings that are stored in the registry.
    /// </summary>
    class RegistrySettings
    {
        private const String RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private const String AutoStartKey = @"FlexTimeMonitor";
        private const String AutoStartTarget = @"AppData\Roaming\Microsoft\Windows\Start Menu\Programs\A9N\FlexTimeMonitor.appref-ms";

        private readonly String autoStartValue;

        public RegistrySettings()
        {

            try
            {
                // The application path changes for every installation. The only way to start it from the current
                // location is to start it via the start menu entry.
                var userProfilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

                this.autoStartValue = Path.Combine(userProfilePath, AutoStartTarget);
            }
            catch (Exception e)
            {
                // TODO: Log this
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether FlexTimeMonitor is automatically started when the user logs in.
        /// </summary>
        /// <value><c>true</c> if FlexTimeMonitor is automatically started; otherwise, <c>false</c>.</value>
        public bool AutoStart
        {
            get
            {
                try
                {
                    var subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey);

                    if (subKey != null)
                    {
                        var result = subKey.GetValue(AutoStartKey, String.Empty);

                        return autoStartValue.Equals(result);
                    }
                }
                catch (Exception exception)
                {
                    // TODO: Log this
                }

                return false;
            }
            set
            {
                try
                {
                    var subKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RunKey, true);

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
                catch (Exception exception)
                {
                    // TODO: Log this
                }
            }
        }
    }
}
