using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace A9N.FlexTimeMonitor.Facility
{
    internal static class SingleInstance
    {
        private static Mutex firstInstanceMutex;

        private enum SW
        {
            /// <summary>
            /// Hides the window and activates another window.
            /// </summary>
            HIDE = 0,
            /// <summary>
            /// Activates and displays a window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when displaying the window for the first time.
            /// </summary>
            SHOWNORMAL = 1,
            /// <summary>
            /// Activates the window and displays it as a minimized window.
            /// </summary>
            SHOWMINIMIZED = 2,
            /// <summary>
            /// Activates the window and displays it as a maximized window.
            /// </summary>
            SHOWMAXIMIZED = 3,
            /// <summary>
            /// Displays a window in its most recent size and position. This value is similar to SW_SHOWNORMAL, except that the window is not activated.
            /// </summary>
            SHOWNOACTIVATE = 4,
            /// <summary>
            /// Activates the window and displays it in its current size and position.
            /// </summary>
            SHOW = 5,
            /// <summary>
            /// Minimizes the specified window and activates the next top-level window in the Z order.
            /// </summary>
            MINIMIZE = 6,
            /// <summary>
            /// Displays the window as a minimized window. This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
            /// </summary>
            SHOWMINNOACTIVE = 7,
            /// <summary>
            /// Displays the window in its current size and position. This value is similar to SW_SHOW, except that the window is not activated.
            /// </summary>
            SHOWNA = 8,
            /// <summary>
            /// Activates and displays the window. If the window is minimized or maximized, the system restores it to its original size and position. An application should specify this flag when restoring a minimized window.
            /// </summary>
            RESTORE = 9,
            /// <summary>
            /// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
            /// </summary>
            SHOWDEFAULT = 10,
            /// <summary>
            /// Minimizes a window, even if the thread that owns the window is not responding. This flag should only be used when minimizing windows from a different thread.
            /// </summary>
            FORCEMINIMIZE = 11,
        };

        [DllImport("user32.dll")]
        //[return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        internal static void EnforceSingleInstance(Action closeApplication)
        {
            if (closeApplication is null)
            {
                throw new ArgumentNullException(nameof(closeApplication));
            }

            firstInstanceMutex = new Mutex(true, "FlexTimeMonitorInstanceMutex", out var isFirstInstance);

            if (!isFirstInstance)
            {
                var current = Process.GetCurrentProcess();

                foreach (var process in Process.GetProcessesByName("FlexTimeMonitor"))
                {
                    if (process.Id != current.Id)
                    {
                        SetForegroundWindow(process.MainWindowHandle);
                        ShowWindowAsync(process.MainWindowHandle, (int)SW.SHOW);
                        ShowWindowAsync(process.MainWindowHandle, (int)SW.RESTORE);
                        break;
                    }
                }

                closeApplication();
            }
        }
    }
}
