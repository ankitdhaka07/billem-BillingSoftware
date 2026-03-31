using domain;

public class Bill
{
    public Guid Id { get; set; }

    public string InvoiceNumber { get; set; }
    public List<BillItem> BillItems { get; set; } = new();
    public DateTime Date { get; set; } = DateTime.Now;
    public double TotalTaxableAmount => BillItems.Sum(billItem => billItem.UnitPrice * billItem.Quantity);
    public DateTime SupplyDate { get; set; } = DateTime.Now;
    public string? VehicleNumber { get; set; }
    public Guid CustomerId { get; set; }       // FK — EF needs this
    public Customer ?Customer { get; set; }
    public double CgstPercentage { get; set; } = 0.025;
    public double SgstPercentage { get; set; } = 0.025;
    
    public double CgstAmount =>
        TotalTaxableAmount * CgstPercentage;

    public double SgstAmount =>
        TotalTaxableAmount * SgstPercentage;

    public double TotalAmount =>
        TotalTaxableAmount + CgstAmount + SgstAmount;
}
