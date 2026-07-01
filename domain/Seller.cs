using System;
using System.Collections.Generic;
using System.Linq;

namespace domain;

/// <summary>
/// In-memory snapshot of the seller/company details used across the app
/// (PDF generation, headers, etc.). Values are loaded from the persisted
/// <see cref="CompanySettings"/> at startup and refreshed whenever the
/// admin saves changes on the Company Settings page.
///
/// The defaults below are only used as the initial seed the very first time
/// the app runs before any settings row exists.
/// </summary>
public static class Seller
{
    public static string Name { get; set; } = "Shiv Ganga Minerals";
    public static string Address { get; set; } = "Vill-Hathnoda, Teh-Chomu, Jaipur, Rajasthan";

    public static List<string> PhoneNumbers { get; set; } = new() { "9414060592", "9828074119" };
    public static string GstNo { get; set; } = "08AKPPR6769L2ZY";
    public static string PANNo { get; set; } = "ABBFS4472L";
    public static string TINNo { get; set; } = "08411660482";

    public static BankDetails BankDetails { get; set; } = new BankDetails
    {
        BankName = "State Bank of India",
        AccountNumber = "43620755550",
        IFSCCode = "SBIN00031365",
        BranchLocation = "Jaipur"
    };

    public static string LogoPath { get; set; } = string.Empty;

    public static double CgstPercentage { get; set; } = 0.025;
    public static double SgstPercentage { get; set; } = 0.025;

    public static string TermsAndConditions { get; set; } =
        "1. Goods once sold will not be taken back.\n" +
        "2. Materials as above are received in good condition.\n" +
        "3. Subject to Jaipur Jurisdiction. 4. Royalty Paid.";

    /// <summary>Refresh the in-memory snapshot from a persisted settings row.</summary>
    public static void ApplyFrom(CompanySettings s)
    {
        if (s is null) return;

        Name = s.Name;
        Address = s.Address;
        PhoneNumbers = SplitPhones(s.PhoneNumbers);
        GstNo = s.GstNo;
        PANNo = s.PANNo;
        TINNo = s.TINNo;
        BankDetails = new BankDetails
        {
            BankName = s.BankName,
            AccountNumber = s.AccountNumber,
            IFSCCode = s.IFSCCode,
            BranchLocation = s.BranchLocation
        };
        LogoPath = s.LogoPath;
        CgstPercentage = s.CgstPercentage;
        SgstPercentage = s.SgstPercentage;
        TermsAndConditions = s.TermsAndConditions;
    }

    /// <summary>Build a settings row from the current defaults (used to seed the DB).</summary>
    public static CompanySettings ToSettings() => new()
    {
        Name = Name,
        Address = Address,
        PhoneNumbers = string.Join(", ", PhoneNumbers),
        GstNo = GstNo,
        PANNo = PANNo,
        TINNo = TINNo,
        BankName = BankDetails.BankName,
        AccountNumber = BankDetails.AccountNumber,
        IFSCCode = BankDetails.IFSCCode,
        BranchLocation = BankDetails.BranchLocation,
        LogoPath = LogoPath,
        CgstPercentage = CgstPercentage,
        SgstPercentage = SgstPercentage,
        TermsAndConditions = TermsAndConditions
    };

    public static List<string> SplitPhones(string csv) =>
        string.IsNullOrWhiteSpace(csv)
            ? new List<string>()
            : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
}

public class BankDetails
{
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public string IFSCCode { get; set; }
    public string BranchLocation { get; set; }
}
