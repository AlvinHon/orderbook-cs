
using OrderBook.Models;

namespace OrderBook.DTOs;

/// <summary>
/// Represents the response of placing an order.
/// </summary>
public static class PlaceOrderResponse
{
    /// <summary>
    /// Represents the result of placing an order. It is returned when the quantity of the order has been consumed
    /// by the existing orders in the market. As a result, the updated orders are returned.
    /// </summary>
    public record UpdatedOrders(List<PlaceOrderResult> PlaceOrderResults);

    /// <summary>
    /// Represents the result of placing an order. It is returned when the orders in the market cannot consume the quantity
    /// of the order. As a result, an order is created in the market.
    /// </summary>
    public record CreatedOrder(int Id, OrderType Type, decimal Price, decimal Quantity, DateTime CreatedAt, string CategoryName)
    {
        public CreatedOrder(Order order, string CategoryName) :
            this(order.Id, order.Type, order.Price, order.Quantity, order.CreatedAt, CategoryName)
        { }
    }

    /// <summary>
    /// Represents the result of updating an order. It is returned when the order in the market cannot consume the quantity
    /// of the order. As a result, the quantity of the order is added to the existing order with same price.
    /// </summary>
    public record UpdatedOrder(int Id, OrderType Type, decimal Price, decimal Quantity, DateTime CreatedAt, string CategoryName)
    {
        public UpdatedOrder(Order order, string CategoryName) :
            this(order.Id, order.Type, order.Price, order.Quantity, order.CreatedAt, CategoryName)
        { }
    }

}