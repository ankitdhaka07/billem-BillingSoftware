using domain;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Services;

namespace UI.Features.Billing;

public class QuantityViewModel : ViewModelBase
{
    public ObservableCollection<BillItem> Items { get; }

    public ICommand GenerateBillCommand { get; }

    public QuantityViewModel(ObservableCollection<BillItem> items)
    {
        Items = items;
        GenerateBillCommand = new RelayCommand(GenerateBill);
    }

    private void GenerateBill()
    {
        var customer = new Customer
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Babulal",
            GstNo = "0ADSFS4772L1PQ",
            BillingAddress = "Bhopawas, Rajasthan",
            ShippingAddress = "Bhopawas, Rajasthan"
        };

        var bill = new Bill
        {
            InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmss}",
            BillItems = Items.ToList(),
            Customer = customer
        };

        var filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "bill.pdf");

        BillPdfGenerator.Generate(bill, filePath);

        Process.Start(new ProcessStartInfo
        {
            FileName = filePath,
            UseShellExecute = true
        });
    }
}
