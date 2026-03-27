using System.ComponentModel.DataAnnotations;

public class BillItem
{
    public Guid Id { get; set; }
    public Guid BillId { get; set; } 
    public Guid ?ItemId { get; set; }
    public string ItemName { get; set; } // snapshot 
    public double UnitPrice { get; set; } //snapshot
    public double Quantity { get; set; }
    public double Amount => UnitPrice * Quantity;
}