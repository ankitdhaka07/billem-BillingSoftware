using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UI;

public class ProductSelectionViewModel : ViewModelBase
{
    public ObservableCollection<Item> AvailableItems { get; }
    public ObservableCollection<BillItem> SelectedItems { get; }

    public ICommand AddItemCommand { get; }
    public ICommand NextCommand { get; }

    public ProductSelectionViewModel(Action<ObservableCollection<BillItem>> onNext)
    {
        AvailableItems = new()
        {
            new Item { Name = "Notebook", Price = 100 },
            new Item { Name = "Pen", Price = 10 },
            new Item { Name = "Pencil", Price = 5 }
        };

        SelectedItems = new ObservableCollection<BillItem>();

        AddItemCommand = new RelayCommand<Item>(AddItem);

        NextCommand = new RelayCommand(
            () => onNext(SelectedItems),
            () => SelectedItems.Any()
        );
    }

    private void AddItem(Item item)
    {
        if (SelectedItems.Any(b => b.Item == item))
            return;

        SelectedItems.Add(new BillItem
        {
            Item = item,
            Quantity = 1
        });

        CommandManager.InvalidateRequerySuggested();
    }
}
