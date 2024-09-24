namespace OrderBook.Models;

/// <summary>
/// Entity representing an order.
/// </summary>
public class Order
{
    public int Id { get; set; }
    public OrderType Type { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
}