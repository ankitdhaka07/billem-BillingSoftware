public class Item
{
    public Guid? Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public override string ToString()
        => $"{Name} - ₹{Price}";
}