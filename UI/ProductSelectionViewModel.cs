using System.Collections.ObjectModel;
using System.Windows.Input;

namespace UI;

public class ProductSelectionViewModel : ViewModelBase
{
    public ObservableCollection<Item> AvailableItems { get; }
    public ObservableCollection<BillItem> SelectedItems { get; }

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
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
        RemoveItemCommand = new RelayCommand<BillItem>(RemoveItem);
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
        var removeIteminAvailble = AvailableItems.FirstOrDefault(i => i.Name == item.Name);
        if(removeIteminAvailble!=null)
        AvailableItems.Remove(removeIteminAvailble);
        CommandManager.InvalidateRequerySuggested();
    }
    private void RemoveItem(BillItem billItem)
    {
        var removeIteminSelected = SelectedItems.FirstOrDefault(i => i.Item.Name == billItem.Item.Name);
        if (removeIteminSelected != null)
            SelectedItems.Remove(billItem);
        AvailableItems.Add(billItem.Item);
        CommandManager.InvalidateRequerySuggested();
    }
}
