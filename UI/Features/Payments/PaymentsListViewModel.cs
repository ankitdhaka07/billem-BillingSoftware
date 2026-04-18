using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;

namespace UI.Features.Payments;

/// <summary>Flat display row used by the list view.</summary>
public record PaymentRow(Guid Id, string CustomerName, double AmountPaid, DateTime PaymentDate);

public class PaymentsListViewModel : ViewModelBase
{
    private readonly PaymentRepository _paymentRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly NavigationService _navigation;

    private List<PaymentRow> _allRows = new();

    public ObservableCollection<PaymentRow> Payments { get; } = new();
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
    public ICommand ViewPaymentCommand { get; }
    public ICommand BackCommand { get; }

    public PaymentsListViewModel(PaymentRepository paymentRepo, ICustomerRepository customerRepo, NavigationService navigation)
    {
        _paymentRepo  = paymentRepo;
        _customerRepo = customerRepo;
        _navigation   = navigation;

        ClearFilterCommand = new RelayCommand(() => SelectedCustomerFilter = null);
        ViewPaymentCommand = new RelayCommand<PaymentRow>(row =>
            navigation.Navigate<PaymentDetailsViewModel>(vm => vm.Load(row)));
        BackCommand = new RelayCommand(() => navigation.Navigate<PaymentsViewModel>());

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var payments  = await _paymentRepo.GetAllPayments();
        var customers = await _customerRepo.GetAllAsync();

        var customerMap = customers.ToDictionary(c => c.Id, c => c.Name);

        _allRows = payments
            .Select(p => new PaymentRow(
                p.Id,
                customerMap.TryGetValue(p.CustomerId ?? Guid.NewGuid(), out var name) ? name : "—",
                p.AmountPaid,
                p.PaymentDate))
            .ToList();

        CustomerFilters.Clear();
        foreach (var c in customers.OrderBy(c => c.Name))
            CustomerFilters.Add(c);

        ApplyFilter();
    }

    private void ApplyFilter()
    {
        Payments.Clear();
        var rows = _selectedCustomerFilter == null
            ? _allRows
            : _allRows.Where(r => r.CustomerName == _selectedCustomerFilter.Name);

        foreach (var r in rows)
            Payments.Add(r);
    }
}
