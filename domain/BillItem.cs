public class BillItem
{
    public Item Item { get; set; }
    public double Quantity { get; set; }
    public double Amount => Item.Price * Quantity;
}