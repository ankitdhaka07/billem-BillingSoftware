using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;

namespace UI.Features.Payments;

public class PaymentsViewModel : ViewModelBase
{
    public ICommand RecordPaymentCommand { get; }
    public ICommand ViewPaymentsCommand { get; }
    public ICommand BackCommand { get; }

    public PaymentsViewModel(NavigationService navigation)
    {
        RecordPaymentCommand = new RelayCommand(() => navigation.Navigate<RecordPaymentViewModel>());
        ViewPaymentsCommand  = new RelayCommand(() => navigation.Navigate<PaymentsListViewModel>());
        BackCommand          = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
    }
}
