using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UI;

public class HomeViewModel : ViewModelBase
{
    public ICommand GenerateBillCommand { get; }
    public ICommand ManageItemsCommand { get; }
    public ICommand ManageCustomersCommand { get; }
    public ICommand AboutCommand { get; }

    public HomeViewModel(NavigationService navigationService, ObservableCollection<BillItem> sharedBillItems)
    {
        GenerateBillCommand = new RelayCommand(() => navigationService.Navigate<ProductSelectionViewModel>());
        ManageItemsCommand = new RelayCommand(() => navigationService.Navigate<ManageItemsViewModel>());
        ManageCustomersCommand = new RelayCommand(() => navigationService.Navigate<ManageCustomersViewModel>());
        AboutCommand = new RelayCommand(() => navigationService.Navigate<AboutViewModel>());
    }
}
