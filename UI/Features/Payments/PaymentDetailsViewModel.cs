using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;

namespace UI.Features.Payments;

public class PaymentDetailsViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;

    private string _customerName = string.Empty;
    public string CustomerName
    {
        get => _customerName;
        private set { _customerName = value; OnPropertyChanged(); }
    }

    private double _amountPaid;
    public double AmountPaid
    {
        get => _amountPaid;
        private set { _amountPaid = value; OnPropertyChanged(); }
    }

    private DateTime _paymentDate;
    public DateTime PaymentDate
    {
        get => _paymentDate;
        private set { _paymentDate = value; OnPropertyChanged(); }
    }

    public ICommand BackCommand { get; }

    public PaymentDetailsViewModel(NavigationService navigation)
    {
        _navigation = navigation;
        BackCommand = new RelayCommand(() => navigation.Navigate<PaymentsListViewModel>());
    }

    /// <summary>Called by NavigationService configure action — no extra DB round-trip needed.</summary>
    public void Load(PaymentRow row)
    {
        CustomerName = row.CustomerName;
        AmountPaid   = row.AmountPaid;
        PaymentDate  = row.PaymentDate;
    }
}
