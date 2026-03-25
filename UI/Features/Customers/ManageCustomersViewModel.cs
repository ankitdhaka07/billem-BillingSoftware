using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;

namespace UI.Features.Customers;

public class ManageCustomersViewModel : ViewModelBase
{
    private readonly ICustomerRepository _customerRepo;
    private readonly NavigationService _navigation;

    // ── Collections ───────────────────────────────────────────────
    public ObservableCollection<Customer> Customers { get; } = new();

    // ── Selected customer (bound to list selection in UI) ─────────
    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set { _selectedCustomer = value; OnPropertyChanged(); }
    }

    // ── Form fields ───────────────────────────────────────────────
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set { _name = value; OnPropertyChanged(); }
    }

    private string? _gstNo;
    public string? GstNo
    {
        get => _gstNo;
        set { _gstNo = value; OnPropertyChanged(); }
    }

    private string? _billingAddress;
    public string? BillingAddress
    {
        get => _billingAddress;
        set { _billingAddress = value; OnPropertyChanged(); }
    }

    private string? _shippingAddress;
    public string? ShippingAddress
    {
        get => _shippingAddress;
        set { _shippingAddress = value; OnPropertyChanged(); }
    }

    // ── Commands ──────────────────────────────────────────────────
    public ICommand BackCommand { get; }
    public ICommand AddCommand { get; }
    public ICommand UpdateCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SelectCommand { get; }
    public ICommand ClearCommand { get; }

    public ManageCustomersViewModel(
        ICustomerRepository customerRepo,
        NavigationService navigation)
    {
        _customerRepo = customerRepo;
        _navigation = navigation;

        BackCommand = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
        AddCommand = new RelayCommand(async () => await AddAsync(), () => !string.IsNullOrWhiteSpace(Name));
        UpdateCommand = new RelayCommand(async () => await UpdateAsync(), () => SelectedCustomer != null && !string.IsNullOrWhiteSpace(Name));
        DeleteCommand = new RelayCommand<Customer>(async (c) =>
        {
            if (c is null) return;
            await _customerRepo.DeleteAsync(c.Id);
            Customers.Remove(c);
        });
        
        SelectCommand = new RelayCommand<Customer>(OnCustomerSelected);
        ClearCommand = new RelayCommand(ClearForm);

        _ = LoadAsync();
    }

    // ── Data loading ──────────────────────────────────────────────
    private async Task LoadAsync()
    {
        var all = await _customerRepo.GetAllAsync();
        Customers.Clear();
        foreach (var c in all)
            Customers.Add(c);
    }

    // ── CRUD operations ───────────────────────────────────────────
    private async Task AddAsync()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = Name,
            GstNo = GstNo,
            BillingAddress = BillingAddress,
            ShippingAddress = ShippingAddress
        };

        await _customerRepo.AddAsync(customer);
        Customers.Add(customer);
        ClearForm();
    }

    private async Task UpdateAsync()
    {
        if (SelectedCustomer is null) return;

        SelectedCustomer.Name = Name;
        SelectedCustomer.GstNo = GstNo;
        SelectedCustomer.BillingAddress = BillingAddress;
        SelectedCustomer.ShippingAddress = ShippingAddress;

        await _customerRepo.UpdateAsync(SelectedCustomer);

        // Refresh the list entry so the UI reflects the change
        var index = Customers.IndexOf(SelectedCustomer);
        if (index >= 0)
        {
            Customers.RemoveAt(index);
            Customers.Insert(index, SelectedCustomer);
        }

        ClearForm();
    }

    private async Task DeleteAsync()
    {
        if (SelectedCustomer is null) return;

        await _customerRepo.DeleteAsync(SelectedCustomer.Id);
        Customers.Remove(SelectedCustomer);
        ClearForm();
    }

    // ── Helpers ───────────────────────────────────────────────────
    private void OnCustomerSelected(Customer? customer)
    {
        if (customer is null) return;
        SelectedCustomer = customer;
        Name = customer.Name;
        GstNo = customer.GstNo;
        BillingAddress = customer.BillingAddress;
        ShippingAddress = customer.ShippingAddress;
    }

    private void ClearForm()
    {
        SelectedCustomer = null;
        Name = string.Empty;
        GstNo = null;
        BillingAddress = null;
        ShippingAddress = null;
    }

}
