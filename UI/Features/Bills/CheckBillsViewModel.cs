using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;

namespace UI.Features.Bills;

public class CheckBillsViewModel : ViewModelBase
{
    private readonly IBillRepository _billRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly NavigationService _navigation;

    private List<Bill> _allBills = new();

    public ObservableCollection<Bill> Bills { get; } = new();
    public ObservableCollection<Customer> CustomerFilters { get; } = new();

    private Customer? _selectedCustomerFilter;
    public Customer? SelectedCustomerFilter
    {
        get => _selectedCustomerFilter;
        set
        {
            _selectedCustomerFilter = value;
            OnPropertyChanged();
            ApplyFilter();
        }
    }

    public ICommand ClearFilterCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand ViewBillCommand { get; }

    public CheckBillsViewModel(IBillRepository billRepo, ICustomerRepository customerRepo, NavigationService navigation)
    {
        _billRepo     = billRepo;
        _customerRepo = customerRepo;
        _navigation   = navigation;

        ClearFilterCommand = new RelayCommand(() => SelectedCustomerFilter = null);
        BackCommand        = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
        ViewBillCommand    = new RelayCommand<Bill>(bill =>
            navigation.Navigate<BillDetailsViewModel>(vm => vm.LoadBill(bill.Id)));

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var bills     = await _billRepo.GetAllAsync();
        var customers = await _customerRepo.GetAllAsync();

        _allBills = bills.ToList();

        CustomerFilters.Clear();
        foreach (var c in customers.OrderBy(c => c.Name))
            CustomerFilters.Add(c);

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        Bills.Clear();
        var filtered = _selectedCustomerFilter == null
            ? _allBills
            : _allBills.Where(b => b.CustomerId == _selectedCustomerFilter.Id);

        foreach (var b in filtered)
            Bills.Add(b);
    }
}
