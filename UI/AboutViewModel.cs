using System.Windows.Input;

namespace UI;

public class AboutViewModel : ViewModelBase
{
    public ICommand BackCommand { get; }

    public AboutViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.Navigate<HomeViewModel>());
    }
}
