using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.About;
using UI.Features.Bills;
using UI.Features.Billing;
using UI.Features.Customers;
using UI.Features.Items;
using UI.Features.Payments;

namespace UI.Features.Home;

public class HomeViewModel : ViewModelBase
{
    public ICommand GenerateBillCommand { get; }
    public ICommand ManageItemsCommand { get; }
    public ICommand ManageCustomersCommand { get; }
    public ICommand CheckBillsCommand { get; }
    public ICommand PaymentsCommand { get; }
    public ICommand AboutCommand { get; }

    public HomeViewModel(NavigationService navigationService)
    {
        GenerateBillCommand    = new RelayCommand(() => navigationService.Navigate<ProductSelectionViewModel>());
        ManageItemsCommand     = new RelayCommand(() => navigationService.Navigate<ManageItemsViewModel>());
        ManageCustomersCommand = new RelayCommand(() => navigationService.Navigate<ManageCustomersViewModel>());
        CheckBillsCommand      = new RelayCommand(() => navigationService.Navigate<CheckBillsViewModel>());
        PaymentsCommand        = new RelayCommand(() => navigationService.Navigate<PaymentsViewModel>());
        AboutCommand           = new RelayCommand(() => navigationService.Navigate<AboutViewModel>());
    }
}
