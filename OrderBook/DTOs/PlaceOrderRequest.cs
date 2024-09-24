using System.ComponentModel.DataAnnotations;
using OrderBook.Models;

namespace OrderBook.DTOs;

/// <summary>
/// Represents the request of placing an order.
/// </summary>
/// <param name="CategoryId">The id of the category of the order.</param>
/// <param name="PlaceOrderType">The type of the order.</param>
/// <param name="LimitPrice">The limit price of the order. Null if Market order.</param>
/// <param name="Quantity">The quantity of the order.</param>
public record PlaceOrderRequest(
    [Required] int CategoryId,
    PlaceOrderType PlaceOrderType,
    [Range(1, double.MaxValue)]
    decimal? LimitPrice,
    [Range(1, double.MaxValue)]
    decimal Quantity
);