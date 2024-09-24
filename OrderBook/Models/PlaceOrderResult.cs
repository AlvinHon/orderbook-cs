namespace OrderBook.Models;

public record PlaceOrderResult(int Id, OrderType Type, decimal PriceAt, decimal OriginalQuantity, decimal ConsumedQuantity, string Category)
{

    public static PlaceOrderResult consumeQuantity(ref Order order, decimal consumedQuantity)
    {
        var OriginalQuantity = order.Quantity;
        order.Quantity -= consumedQuantity;
        return new PlaceOrderResult(order.Id, order.Type, order.Price, OriginalQuantity, consumedQuantity, order.Category.Name);
    }
}