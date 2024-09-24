namespace OrderBook.Models;

public class Order
{
    public int Id { get; set; }
    public OrderType Type { get; set; }
    public decimal Price { get; set; }
    public decimal Quantity { get; set; }
    public DateTime CreatedAt { get; set; }

    public required int CategoryId { get; set; }
    public required Category Category { get; set; }
}