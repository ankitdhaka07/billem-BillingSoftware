using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
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

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public ICommand GenerateLedgerCommand { get; }
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
        GenerateLedgerCommand = new RelayCommand(
            async () => await GenerateAsync(),
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

    private async Task GenerateAsync()
    {
        if (SelectedCustomer is null) return;

        StatusMessage = "Generating ledger...";

        var fromDate = FromDate.Date;
        var toDate = ToDate.Date.AddDays(1).AddTicks(-1);

        var allBills = await _billRepo.GetAllAsync();
        var allPayments = await _paymentRepo.GetAllPayments();

        // Opening balance = net of all transactions strictly before the period start
        var priorBills    = allBills.Where(b => b.CustomerId == SelectedCustomer.Id && b.Date < fromDate).Sum(b => b.TotalAmount);
        var priorPayments = allPayments.Where(p => p.CustomerId == SelectedCustomer.Id && p.PaymentDate < fromDate).Sum(p => p.AmountPaid);
        var openingBalance = priorBills - priorPayments; // positive = Dr (customer owes us)

        var bills = allBills
            .Where(b => b.CustomerId == SelectedCustomer.Id
                        && b.Date >= fromDate
                        && b.Date <= toDate)
            .OrderBy(b => b.Date)
            .ToList();

        var payments = allPayments
            .Where(p => p.CustomerId == SelectedCustomer.Id
                        && p.PaymentDate >= fromDate
                        && p.PaymentDate <= toDate)
            .OrderBy(p => p.PaymentDate)
            .ToList();

        var fileName = $"Ledger_{SelectedCustomer.Name.Replace(" ", "_")}_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}.pdf";
        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            fileName);

        LedgerPdfGenerator.Generate(SelectedCustomer, FromDate, ToDate, bills, payments, openingBalance, filePath);

        StatusMessage = $"Saved: {fileName}";

        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
    }
}
