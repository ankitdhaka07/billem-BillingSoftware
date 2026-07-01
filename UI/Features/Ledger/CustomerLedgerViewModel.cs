using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;
using UI.Services;

namespace UI.Features.Ledger;

public class CustomerLedgerViewModel : ViewModelBase
{
    private readonly IBillRepository _billRepo;
    private readonly PaymentRepository _paymentRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly NavigationService _navigation;

    public ObservableCollection<Customer> Customers { get; } = new();

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set { _selectedCustomer = value; OnPropertyChanged(); }
    }

    private DateTime _fromDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
    public DateTime FromDate
    {
        get => _fromDate;
        set { _fromDate = value; OnPropertyChanged(); }
    }

    private DateTime _toDate = DateTime.Today;
    public DateTime ToDate
    {
        get => _toDate;
        set { _toDate = value; OnPropertyChanged(); }
    }

    // Off by default — the ledger is shown in the UI; PDF only when asked for.
    private bool _alsoGeneratePdf;
    public bool AlsoGeneratePdf
    {
        get => _alsoGeneratePdf;
        set { _alsoGeneratePdf = value; OnPropertyChanged(); }
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public ICommand ViewLedgerCommand { get; }
    public ICommand BackCommand { get; }

    public CustomerLedgerViewModel(
        IBillRepository billRepo,
        PaymentRepository paymentRepo,
        ICustomerRepository customerRepo,
        NavigationService navigation)
    {
        _billRepo = billRepo;
        _paymentRepo = paymentRepo;
        _customerRepo = customerRepo;
        _navigation = navigation;

        BackCommand = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
        ViewLedgerCommand = new RelayCommand(
            async () => await ViewLedgerAsync(),
            () => SelectedCustomer != null && FromDate <= ToDate);

        _ = LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        var all = await _customerRepo.GetAllAsync();
        Customers.Clear();
        foreach (var c in all)
            Customers.Add(c);
    }

    private async Task ViewLedgerAsync()
    {
        if (SelectedCustomer is null) return;

        StatusMessage = "Calculating ledger...";

        var allBills = await _billRepo.GetAllAsync();
        var allPayments = await _paymentRepo.GetAllPayments();

        var ledger = LedgerCalculator.Calculate(SelectedCustomer, FromDate, ToDate, allBills, allPayments);

        if (AlsoGeneratePdf)
        {
            var filePath = LedgerPdfGenerator.ExportToDesktop(ledger);
            Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
        }

        StatusMessage = string.Empty;
        _navigation.Navigate<LedgerDetailsViewModel>(vm => vm.Load(ledger));
    }
}
