public class Item
{
    public string Name { get; set; }
    public int Price { get; set; }
    public override string ToString()
        => $"{Name} - ₹{Price}";
}