public class Bill
{
    public Guid Id { get; set; }
    public List<BillItem> BillItems { get; set; } = new();
    public DateTime Date { get; set; } = DateTime.Now;
    public double TotalTaxableAmount  => BillItems.Sum(billItem => billItem.Item.Price * billItem.Quantity);
    public double TaxPercentage = 0.05;
    public double TaxApplied => TotalTaxableAmount * TaxPercentage;
    public double TotalAmount => TotalTaxableAmount * 1.05;
    public string VehicleNumber { get; set; }
}