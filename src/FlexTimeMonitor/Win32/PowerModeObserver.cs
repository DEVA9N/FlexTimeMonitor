using Microsoft.Win32;
using System;

namespace A9N.FlexTimeMonitor.Win32
{
    internal sealed class PowerModeObserver
    {
        private Action save;

        public PowerModeObserver(Action save)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            SystemEvents.PowerModeChanged += SystemEvents_PowerModeChanged;
#pragma warning restore CA1416 // Validate platform compatibility

            this.save = save;
        }

        private void SystemEvents_PowerModeChanged(object sender, PowerModeChangedEventArgs e)
        {
#pragma warning disable CA1416 // Validate platform compatibility
            if (e.Mode == PowerModes.Suspend)
#pragma warning restore CA1416 // Validate platform compatibility
            {
                save();
            }
        }
    }
}
