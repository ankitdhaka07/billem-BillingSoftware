public class Item
{
    public string Name { get; set; }
    public double Price { get; set; }
    public override string ToString()
        => $"{Name} - ₹{Price}";
}