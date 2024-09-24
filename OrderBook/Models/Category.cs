namespace OrderBook.Models;

public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Order> Orders { get; } = new();
}