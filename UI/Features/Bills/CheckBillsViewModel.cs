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
    private readonly NavigationService _navigation;

    public ObservableCollection<Bill> Bills { get; } = new();

    public ICommand BackCommand { get; }

    public CheckBillsViewModel(IBillRepository billRepo, NavigationService navigation)
    {
        _billRepo = billRepo;
        _navigation = navigation;
        BackCommand = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
        _ = LoadAsync();
    }

    private async Task LoadAsync()
    {
        var all = await _billRepo.GetAllAsync();
        Bills.Clear();
        foreach (var b in all)
            Bills.Add(b);
    }
}
