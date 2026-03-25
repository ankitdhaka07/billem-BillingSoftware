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

    public MainWindowViewModel(NavigationService nav)
    {
        nav.OnNavigate = vm => CurrentViewModel = vm;
        nav.Navigate<HomeViewModel>();
    }
}
