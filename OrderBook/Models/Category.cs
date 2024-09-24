namespace OrderBook.Models;

/// <summary>
/// Entity representing a category which contains a list of orders.
/// </summary>
public class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Order> Orders { get; } = new();
}