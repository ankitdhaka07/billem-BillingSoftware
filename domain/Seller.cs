using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain;

public static class Seller
{
    public static string Name { get; set; } = "Shiv Ganga Minerals";
    public static string FirmOfficeAddressLine1 { get; set; } = "Vill-Hathnoda, Teh-Chomu, Jaipur, Rajasthan";
    public static string FirmOfficeAddressLine2 { get; set; } = " Vill-Hathnoda, Teh-Chomu, Jaipur, Rajasthan";

    public static List<string> PhoneNumbers { get; set; } = new List<string> { "9414060592", "9828074119" };
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
    public static string LogoPath { get; set; } =
        "C:\\Users\\ankit\\Chanagble\\Personal\\SDE\\Playground\\billem-BillingSoftware\\UI\\Static\\ShivGangaLogo.jpeg";
}
public class BankDetails
{
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public string IFSCCode { get; set; }
    public string BranchLocation { get; set; }
}