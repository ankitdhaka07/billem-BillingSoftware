public class BillItem
{
    public Item Item { get; set; }
    public int Quantity { get; set; }
    public int Amount => Item.Price * Quantity;
}