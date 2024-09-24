using OrderBook.Models;

namespace OrderBook.DTOs;

/// <summary>
/// Represents the response of getting an order.
/// </summary>
/// <param name="Id">The id of the order.</param>
/// <param name="type">The type of the order.</param>
/// <param name="Price">The price of the order.</param>
/// <param name="Quantity">The quantity of the order.</param>
/// <param name="CreatedAt">The creation time of the order.</param>
/// <param name="Category">The category of the order.</param>
public record GetOrderResponse(int Id, OrderType type, decimal Price, decimal Quantity, DateTime CreatedAt, string Category);