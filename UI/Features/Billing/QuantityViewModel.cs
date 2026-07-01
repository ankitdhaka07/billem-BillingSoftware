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

namespace UI.Features.Billing;

public class QuantityViewModel : ViewModelBase
{
    private readonly BillingSession _session;
    private readonly NavigationService _navigation;
    private readonly IBillRepository _billRepo;

    public ObservableCollection<BillItem> Items => _session.SelectedItems;

    public ICommand GenerateBillCommand { get; }
    public ICommand BackCommand { get; }

    public QuantityViewModel(BillingSession session, NavigationService navigation, IBillRepository billRepo)
    {
        _session = session;
        _navigation = navigation;
        _billRepo = billRepo;
        GenerateBillCommand = new RelayCommand(async () => await GenerateBillAsync());
        BackCommand = new RelayCommand(
            () => navigation.Navigate<ProductSelectionViewModel>());
    }

    private async Task GenerateBillAsync()
    {
        var customer = _session.SelectedCustomer!;

        var bill = new Bill
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
            BillItems = Items.ToList(),
            Customer = customer,
            CustomerId = customer.Id,
            Date = DateTime.Now,
            CgstPercentage = Seller.CgstPercentage,
            SgstPercentage = Seller.SgstPercentage
        };

        await _billRepo.AddAsync(bill);

        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            $"{bill.InvoiceNumber}.pdf");

        BillPdfGenerator.Generate(bill, filePath);

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });

        _navigation.Navigate<HomeViewModel>();
    }
}
