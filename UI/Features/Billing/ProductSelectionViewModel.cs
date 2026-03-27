using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;

namespace UI.Features.Billing;

public class ProductSelectionViewModel : ViewModelBase
{
    private readonly BillingSession _session;
    private Customer? _selectedCustomer;

    public ObservableCollection<Customer> Customers { get; } = new();
    public ObservableCollection<Item> AvailableItems { get; set; }
    public ObservableCollection<BillItem> SelectedItems => _session.SelectedItems;

    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            _session.SelectedCustomer = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    public ICommand AddItemCommand { get; }
    public ICommand RemoveItemCommand { get; }
    public ICommand NextCommand { get; }

    public ProductSelectionViewModel(NavigationService navigationService, BillingSession session, ICustomerRepository customerRepository, IItemRepository itemRepository)
    {
        _session = session;
        _session.SelectedItems.Clear();

        AvailableItems = new();

        AddItemCommand = new RelayCommand<Item>(AddItem);
        RemoveItemCommand = new RelayCommand<BillItem>(RemoveItem);
        NextCommand = new RelayCommand(
            () => navigationService.Navigate<QuantityViewModel>(),
            () => SelectedItems.Any() && SelectedCustomer != null
        );

        _ = LoadCustomersAsync(customerRepository);
        _ = LoadItemsAsync(itemRepository);
    }

    private async Task LoadCustomersAsync(ICustomerRepository repo)
    {
        var customers = await repo.GetAllAsync();
        foreach (var c in customers)
            Customers.Add(c);
    }

    private async Task LoadItemsAsync(IItemRepository repo)
    {
        var items = await repo.GetAllAsync();
        foreach (var i in items)
            AvailableItems.Add(i);
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

        var removeItemInAvailable = AvailableItems.FirstOrDefault(i => i.Name == item.Name);
        if (removeItemInAvailable != null)
            AvailableItems.Remove(removeItemInAvailable);
        CommandManager.InvalidateRequerySuggested();
    }

    private void RemoveItem(BillItem billItem)
    {
        SelectedItems.Remove(billItem);
        AvailableItems.Add(new Item { Id = billItem.ItemId == Guid.Empty ? null : billItem.ItemId, Name = billItem.ItemName, Price = billItem.UnitPrice });
        CommandManager.InvalidateRequerySuggested();
    }
}
