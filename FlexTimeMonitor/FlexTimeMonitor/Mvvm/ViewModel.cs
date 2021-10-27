using System.ComponentModel;

namespace A9N.FlexTimeMonitor.Mvvm
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        // Implemented by PropertyChanged.Fody
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
