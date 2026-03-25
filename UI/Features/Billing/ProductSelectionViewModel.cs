using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;

namespace UI.Features.Billing;

public class ProductSelectionViewModel : ViewModelBase
{
    public ObservableCollection<Item> AvailableItems { get; }
    public ObservableCollection<BillItem> SelectedItems { get; }

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand NextCommand { get; }

    public ProductSelectionViewModel(NavigationService navigationService, ObservableCollection<BillItem> selectedItems)
    {
        AvailableItems = new()
        {
            new Item { Name = "Notebook", Price = 100 },
            new Item { Name = "Pen", Price = 10 },
            new Item { Name = "Pencil", Price = 5 }
        };

        SelectedItems = selectedItems;

        AddItemCommand = new RelayCommand<Item>(AddItem);
        RemoveItemCommand = new RelayCommand<BillItem>(RemoveItem);
        NextCommand = new RelayCommand(
            () => navigationService.Navigate<QuantityViewModel>(),
            () => SelectedItems.Any()
        );
    }

    private void AddItem(Item item)
    {
        if (SelectedItems.Any(b => b.ItemName == item.Name))
            return;

        SelectedItems.Add(new BillItem
        {
            ItemId = item.Id ?? Guid.NewGuid(),
            ItemName = item.Name,
            UnitPrice = item.Price,
            Quantity = 1
        });
        var removeIteminAvailble = AvailableItems.FirstOrDefault(i => i.Name == item.Name);
        if (removeIteminAvailble != null)
            AvailableItems.Remove(removeIteminAvailble);
        CommandManager.InvalidateRequerySuggested();
    }

    private void RemoveItem(BillItem billItem)
    {
        SelectedItems.Remove(billItem);
        AvailableItems.Add(new Item { Id = billItem.ItemId == Guid.Empty ? null : billItem.ItemId, Name = billItem.ItemName, Price = billItem.UnitPrice });
        CommandManager.InvalidateRequerySuggested();
    }
}
