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
        ShowProductSelection();
    }

    private void ShowProductSelection()
    {
        CurrentViewModel = new ProductSelectionViewModel(OnProductSelected);
    }

    private void OnProductSelected(ObservableCollection<BillItem> items)
    {
        CurrentViewModel = new QuantityViewModel(items);
    }

}
