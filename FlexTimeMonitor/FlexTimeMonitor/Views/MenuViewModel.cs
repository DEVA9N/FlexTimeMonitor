using System;
using System.Windows;
using System.Windows.Input;
using A9N.FlexTimeMonitor.Mvvm;

namespace A9N.FlexTimeMonitor.Views
{
    internal sealed class MenuViewModel : ViewModel
    {
        public ICommand ShowAbout { get; }
        public ICommand ShowOptions { get; }

        internal MenuViewModel(Window window)
        {
            if (window == null) throw new ArgumentNullException(nameof(window));

            // Save and Quit are implemented as CommandBinding in order to make use of InputBindings (e.g. Ctrl+S)

            ShowAbout = CreateShowAboutCommand(window);

            ShowOptions = CreateShowOptionsCommand(window);
        }

        private ICommand CreateShowAboutCommand(Window owner)
        {
            return new RelayCommand(() =>
            {
                var about = new AboutDialog()
                {
                    Owner = owner,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                about.ShowDialog();
            });
        }

        private ICommand CreateShowOptionsCommand(Window owner)
        {
            return new RelayCommand(() =>
            {
                var options = new OptionsDialog()
                {
                    Owner = owner,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                options.ShowDialog();
            });
        }
    }
}