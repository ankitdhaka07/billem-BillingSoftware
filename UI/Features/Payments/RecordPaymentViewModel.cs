using domain;
using Infrastructure;
using System.Collections.ObjectModel;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;

namespace UI.Features.Payments;

public class RecordPaymentViewModel : ViewModelBase
{
    private readonly PaymentRepository _paymentRepo;
    private readonly ICustomerRepository _customerRepo;
    private readonly NavigationService _navigation;

    public ObservableCollection<Customer> Customers { get; } = new();

    private Customer? _selectedCustomer;
    public Customer? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            _selectedCustomer = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private string _amountText = string.Empty;
    public string AmountText
    {
        get => _amountText;
        set
        {
            _amountText = value;
            OnPropertyChanged();
            CommandManager.InvalidateRequerySuggested();
        }
    }

    private DateTime _paymentDate = DateTime.Today;
    public DateTime PaymentDate
    {
        get => _paymentDate;
        set { _paymentDate = value; OnPropertyChanged(); }
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    public RecordPaymentViewModel(PaymentRepository paymentRepo, ICustomerRepository customerRepo, NavigationService navigation)
    {
        _paymentRepo  = paymentRepo;
        _customerRepo = customerRepo;
        _navigation   = navigation;

        SaveCommand   = new RelayCommand(ExecuteSave, CanSave);
        CancelCommand = new RelayCommand(() => navigation.Navigate<PaymentsViewModel>());

        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var customers = await _customerRepo.GetAllAsync();
        Customers.Clear();
        foreach (var c in customers)
            Customers.Add(c);
    }

    private bool CanSave() =>
        SelectedCustomer != null &&
        double.TryParse(AmountText, out var a) && a > 0;

    private void ExecuteSave()
    {
        if (!double.TryParse(AmountText, out var amount) || SelectedCustomer == null)
            return;

        _ = SaveAsync(new Payment
        {
            CustomerId  = SelectedCustomer.Id,
            AmountPaid  = amount,
            PaymentDate = PaymentDate
        });
    }

    private async Task SaveAsync(Payment payment)
    {
        await _paymentRepo.AddPayment(payment);
        _navigation.Navigate<PaymentsViewModel>();
    }
}
