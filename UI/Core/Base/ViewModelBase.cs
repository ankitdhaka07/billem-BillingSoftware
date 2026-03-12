using System.ComponentModel;

namespace UI.Core.Base;

public class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged (string propertyName) => PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (propertyName));
}