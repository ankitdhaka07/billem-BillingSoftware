using System.Collections.ObjectModel;
using UI.Core.Base;
using UI.Core.Navigation;
using UI.Features.About;
using UI.Features.Billing;
using UI.Features.Customers;
using UI.Features.Home;
using UI.Features.Items;

namespace UI;

public class MainWindowViewModel : ViewModelBase
{
    private ViewModelBase _currentViewModel;
    public ViewModelBase CurrentViewModel
    {
        get => _currentViewModel;
        set
        {
            _currentViewModel = value;
            OnPropertyChanged(nameof(CurrentViewModel));
        }
    }

    public MainWindowViewModel()
    {
        var selectedItems = new ObservableCollection<BillItem>();

        NavigationService nav = null!;
        nav = new NavigationService(type =>
        {
            if (type == typeof(HomeViewModel))
                return new HomeViewModel(nav, selectedItems);
            if (type == typeof(ProductSelectionViewModel))
                return new ProductSelectionViewModel(nav, selectedItems);
            if (type == typeof(QuantityViewModel))
                return new QuantityViewModel(selectedItems);
            if (type == typeof(ManageItemsViewModel))
                return new ManageItemsViewModel(nav);
            if (type == typeof(ManageCustomersViewModel))
                return new ManageCustomersViewModel(nav);
            if (type == typeof(AboutViewModel))
                return new AboutViewModel(nav);
            throw new InvalidOperationException($"No factory registered for {type.Name}");
        });

        nav.OnNavigate = vm => CurrentViewModel = vm;
        nav.Navigate<HomeViewModel>();
    }
}
