using domain;
using Infrastructure;
using System.Globalization;
using Microsoft.Win32;
using System.Windows.Input;
using UI.Core.Base;
using UI.Core.Commands;
using UI.Core.Navigation;
using UI.Features.Home;

namespace UI.Features.Settings;

public class CompanySettingsViewModel : ViewModelBase
{
    private readonly ICompanySettingsRepository _repo;
    private readonly NavigationService _navigation;

    private Guid _id;

    public CompanySettingsViewModel(ICompanySettingsRepository repo, NavigationService navigation)
    {
        _repo = repo;
        _navigation = navigation;

        SaveCommand = new RelayCommand(async () => await SaveAsync(), () => !string.IsNullOrWhiteSpace(Name));
        BackCommand = new RelayCommand(() => navigation.Navigate<HomeViewModel>());
        BrowseLogoCommand = new RelayCommand(BrowseLogo);

        _ = LoadAsync();
    }

    // ── Commands ──────────────────────────────────────────────────
    public ICommand SaveCommand { get; }
    public ICommand BackCommand { get; }
    public ICommand BrowseLogoCommand { get; }

    // ── Form fields ───────────────────────────────────────────────
    private string _name = string.Empty;
    public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

    private string _address = string.Empty;
    public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

    private string _phoneNumbers = string.Empty;
    public string PhoneNumbers { get => _phoneNumbers; set { _phoneNumbers = value; OnPropertyChanged(); } }

    private string _gstNo = string.Empty;
    public string GstNo { get => _gstNo; set { _gstNo = value; OnPropertyChanged(); } }

    private string _panNo = string.Empty;
    public string PANNo { get => _panNo; set { _panNo = value; OnPropertyChanged(); } }

    private string _tinNo = string.Empty;
    public string TINNo { get => _tinNo; set { _tinNo = value; OnPropertyChanged(); } }

    private string _bankName = string.Empty;
    public string BankName { get => _bankName; set { _bankName = value; OnPropertyChanged(); } }

    private string _accountNumber = string.Empty;
    public string AccountNumber { get => _accountNumber; set { _accountNumber = value; OnPropertyChanged(); } }

    private string _ifscCode = string.Empty;
    public string IFSCCode { get => _ifscCode; set { _ifscCode = value; OnPropertyChanged(); } }

    private string _branchLocation = string.Empty;
    public string BranchLocation { get => _branchLocation; set { _branchLocation = value; OnPropertyChanged(); } }

    private string _logoPath = string.Empty;
    public string LogoPath { get => _logoPath; set { _logoPath = value; OnPropertyChanged(); } }

    // Tax rates are shown to the admin as percentages (e.g. "2.5"), stored as fractions.
    private string _cgstPercent = string.Empty;
    public string CgstPercent { get => _cgstPercent; set { _cgstPercent = value; OnPropertyChanged(); } }

    private string _sgstPercent = string.Empty;
    public string SgstPercent { get => _sgstPercent; set { _sgstPercent = value; OnPropertyChanged(); } }

    private string _termsAndConditions = string.Empty;
    public string TermsAndConditions { get => _termsAndConditions; set { _termsAndConditions = value; OnPropertyChanged(); } }

    private string _statusMessage = string.Empty;
    public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnPropertyChanged(); } }

    private async Task LoadAsync()
    {
        var s = await _repo.GetAsync();
        _id = s.Id;

        Name = s.Name;
        Address = s.Address;
        PhoneNumbers = s.PhoneNumbers;
        GstNo = s.GstNo;
        PANNo = s.PANNo;
        TINNo = s.TINNo;
        BankName = s.BankName;
        AccountNumber = s.AccountNumber;
        IFSCCode = s.IFSCCode;
        BranchLocation = s.BranchLocation;
        LogoPath = s.LogoPath;
        CgstPercent = (s.CgstPercentage * 100).ToString(CultureInfo.InvariantCulture);
        SgstPercent = (s.SgstPercentage * 100).ToString(CultureInfo.InvariantCulture);
        TermsAndConditions = s.TermsAndConditions;
    }

    private void BrowseLogo()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select company logo",
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp)|*.png;*.jpg;*.jpeg;*.bmp|All files (*.*)|*.*"
        };
        if (dialog.ShowDialog() == true)
            LogoPath = dialog.FileName;
    }

    private async Task SaveAsync()
    {
        var settings = new CompanySettings
        {
            Id = _id,
            Name = Name?.Trim() ?? string.Empty,
            Address = Address?.Trim() ?? string.Empty,
            PhoneNumbers = PhoneNumbers?.Trim() ?? string.Empty,
            GstNo = GstNo?.Trim() ?? string.Empty,
            PANNo = PANNo?.Trim() ?? string.Empty,
            TINNo = TINNo?.Trim() ?? string.Empty,
            BankName = BankName?.Trim() ?? string.Empty,
            AccountNumber = AccountNumber?.Trim() ?? string.Empty,
            IFSCCode = IFSCCode?.Trim() ?? string.Empty,
            BranchLocation = BranchLocation?.Trim() ?? string.Empty,
            LogoPath = LogoPath?.Trim() ?? string.Empty,
            CgstPercentage = ParsePercent(CgstPercent),
            SgstPercentage = ParsePercent(SgstPercent),
            TermsAndConditions = TermsAndConditions ?? string.Empty
        };

        await _repo.SaveAsync(settings);

        // Refresh the in-memory snapshot so PDFs/headers pick up changes immediately.
        Seller.ApplyFrom(settings);

        StatusMessage = "Settings saved.";
    }

    /// <summary>Parses a percentage entered by the admin (e.g. "2.5") into a fraction (0.025).</summary>
    private static double ParsePercent(string text)
        => double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out var p) ? p / 100.0 : 0;
}
