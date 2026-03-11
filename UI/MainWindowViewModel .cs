using System.Collections.ObjectModel;

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
            if (type == typeof(ProductSelectionViewModel))
                return new ProductSelectionViewModel(nav, selectedItems);
            if (type == typeof(QuantityViewModel))
                return new QuantityViewModel(selectedItems);
            throw new InvalidOperationException($"No factory registered for {type.Name}");
        });

        nav.OnNavigate = vm => CurrentViewModel = vm;
        nav.Navigate<ProductSelectionViewModel>();
    }
}
