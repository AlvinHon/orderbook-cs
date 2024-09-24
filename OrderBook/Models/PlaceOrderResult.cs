namespace OrderBook.Models;

/// <summary>
/// Represents the result of placing an order.
/// </summary>
public record PlaceOrderResult(int Id, OrderType Type, decimal PriceAt, decimal OriginalQuantity, decimal ConsumedQuantity, string Category)
{
    public PlaceOrderResult(Order order, decimal consumedQuantity, string Category) : this(
        order.Id, order.Type, order.Price, order.Quantity, consumedQuantity, Category
    )
    {
    }
}