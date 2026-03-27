using domain;
using Infrastructure;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;

namespace UI.Features.Bills;

public class BillDetailsViewModel : ViewModelBase
{
    private readonly IBillRepository _billRepo;
    private readonly NavigationService _navigation;

    private Bill? _bill;
    public Bill? Bill
    {
        get => _bill;
        private set { _bill = value; OnPropertyChanged(); }
    }

    public ICommand BackCommand { get; }

    public BillDetailsViewModel(IBillRepository billRepo, NavigationService navigation)
    {
        _billRepo = billRepo;
        _navigation = navigation;
        BackCommand = new RelayCommand(() => navigation.Navigate<CheckBillsViewModel>());
    }

    public void LoadBill(Guid billId)
    {
        _ = LoadAsync(billId);
    }

    private async Task LoadAsync(Guid billId)
    {
        Bill = await _billRepo.GetByIdAsync(billId);
    }
}
