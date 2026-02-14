using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace UI;

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
        var bill = new Bill
        {
            BillItems = Items.ToList()
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
