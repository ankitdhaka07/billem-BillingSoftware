using System.Diagnostics;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Services;

namespace UI.Features.Ledger;

public class LedgerDetailsViewModel : ViewModelBase
{
    private readonly NavigationService _navigation;

    private LedgerResult? _ledger;
    public LedgerResult? Ledger
    {
        get => _ledger;
        private set { _ledger = value; OnPropertyChanged(); }
    }

    private string _statusMessage = string.Empty;
    public string StatusMessage
    {
        get => _statusMessage;
        set { _statusMessage = value; OnPropertyChanged(); }
    }

    public ICommand GeneratePdfCommand { get; }
    public ICommand BackCommand { get; }

    public LedgerDetailsViewModel(NavigationService navigation)
    {
        _navigation = navigation;

        BackCommand = new RelayCommand(() => navigation.Navigate<CustomerLedgerViewModel>());
        GeneratePdfCommand = new RelayCommand(GeneratePdf, () => Ledger != null);
    }

    public void Load(LedgerResult ledger)
    {
        Ledger = ledger;
    }

    private void GeneratePdf()
    {
        if (Ledger is null) return;

        var filePath = LedgerPdfGenerator.ExportToDesktop(Ledger);
        StatusMessage = $"Saved: {System.IO.Path.GetFileName(filePath)}";

        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
    }
}
