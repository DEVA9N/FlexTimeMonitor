using System.ComponentModel;

namespace A9N.FlexTimeMonitor.Mvvm
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        // Disable warning for code implemented by PropertyChanged.Fody
#pragma warning disable CS0067 
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore  CS0067
    }
}
