using System.Windows.Input;

namespace UI;

public class ManageItemsViewModel : ViewModelBase
{
    public ICommand BackCommand { get; }

    public ManageItemsViewModel(NavigationService navigationService)
    {
        BackCommand = new RelayCommand(() => navigationService.Navigate<HomeViewModel>());
    }
}
