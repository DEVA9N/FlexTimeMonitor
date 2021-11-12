using A9N.FlexTimeMonitor.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace A9N.FlexTimeMonitor.Infrastructure
{
    internal static class SingleInstance
    {
        private static Mutex firstInstanceMutex;

        internal static bool CheckIsFirstInstance()
        {
            firstInstanceMutex = new Mutex(true, "FlexTimeMonitorInstanceMutex", out var isFirstInstance);

            if (!isFirstInstance)
            {
                ShowOtherInstance();
            }

            return isFirstInstance;
        }

        private static void ShowOtherInstance()
        {
            var current = Process.GetCurrentProcess();

            var otherInstance = Process.GetProcessesByName("FlexTimeMonitor").Where(p => p.Id != current.Id).FirstOrDefault();

            if (otherInstance != null)
            {
                Window.Show(otherInstance);
            }
        }
    }
}
