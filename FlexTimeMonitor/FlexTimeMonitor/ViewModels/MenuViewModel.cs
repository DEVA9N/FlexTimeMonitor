using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using A9N.FlexTimeMonitor.Windows;

namespace A9N.FlexTimeMonitor.ViewModels
{
    internal sealed class MenuViewModel : ViewModelBase
    {
        public ICommand Save { get; }
        public ICommand Quit { get; }
        public ICommand ShowAbout { get; }
        public ICommand ShowOptions { get; }

        internal MenuViewModel(MainWindow window)
        {
            Save = new RelayCommand(window.Save);
            Quit = new RelayCommand(window.Close);
            ShowAbout = new RelayCommand(() =>
            {
                var about = new AboutWindow
                {
                    Owner = window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                about.ShowDialog();
            });
            ShowOptions = new RelayCommand(() =>
            {
                var options = new OptionsWindow
                {
                    Owner = window,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                options.ShowDialog();
            });
        }

        private class RelayCommand : ICommand
        {
            private readonly Action _execute;

            public event EventHandler CanExecuteChanged
            {
                add => CommandManager.RequerySuggested += value;
                remove => CommandManager.RequerySuggested -= value;
            }

            public RelayCommand(Action execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                _execute();
            }
        }
    }
}