namespace domain;

/// <summary>
/// Persisted, admin-editable company/seller configuration.
/// A single row lives in the database; edit it via the Company Settings page.
/// </summary>
public class CompanySettings
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;

    /// <summary>Comma-separated phone numbers.</summary>
    public string PhoneNumbers { get; set; } = string.Empty;

    public string GstNo { get; set; } = string.Empty;
    public string PANNo { get; set; } = string.Empty;
    public string TINNo { get; set; } = string.Empty;

    public string BankName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string IFSCCode { get; set; } = string.Empty;
    public string BranchLocation { get; set; } = string.Empty;

    public string LogoPath { get; set; } = string.Empty;

    /// <summary>Stored as a fraction, e.g. 0.025 for 2.5%.</summary>
    public double CgstPercentage { get; set; } = 0.025;
    public double SgstPercentage { get; set; } = 0.025;

    public string TermsAndConditions { get; set; } = string.Empty;
}
