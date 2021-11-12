using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using A9N.FlexTimeMonitor.Mvvm;
using A9N.FlexTimeMonitor.Properties;
using A9N.FlexTimeMonitor.Win32;

namespace A9N.FlexTimeMonitor.Views
{
    internal class OptionsViewModel : ViewModel
    {
        private readonly RegistrySettings _registry;

        public ICommand AcceptCommand { get; }
        public ICommand CancelCommand { get; }
        public bool AutoStart { get; set; }
        public TimeSpan BreakPeriod { get; set; }
        public TimeSpan WorkPeriod { get; set; }

        public OptionsViewModel(Action close)
        {
            if (close == null) throw new ArgumentNullException(nameof(close));

            // Can't bind AutoStart directly to this setting or else Cancel does not affect this setting
            _registry = new RegistrySettings();

            AutoStart = _registry.AutoStart;
            BreakPeriod = Settings.Default.BreakPeriod;
            WorkPeriod = Settings.Default.WorkPeriod;

            AcceptCommand = new RelayCommand(SaveValues);
            CancelCommand = new RelayCommand(close);
        }

        private void SaveValues()
        {
            _registry.AutoStart = AutoStart;
           
            Settings.Default.WorkPeriod = WorkPeriod;
            Settings.Default.BreakPeriod = BreakPeriod;
            Settings.Default.Save();
        }

    }
}