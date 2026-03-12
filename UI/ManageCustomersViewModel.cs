using System.Windows.Input;

namespace UI;

public class ManageCustomersViewModel : ViewModelBase
{
    public ICommand BackCommand { get; }

    public ManageCustomersViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.Navigate<HomeViewModel>());
    }
}
