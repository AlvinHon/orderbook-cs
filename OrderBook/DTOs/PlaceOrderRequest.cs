using System.ComponentModel.DataAnnotations;
using OrderBook.Models;

namespace OrderBook.DTOs;

public record PlaceOrderRequest(
    [Required] int CategoryId,
    PlaceOrderType PlaceOrderType,
    [Range(1, double.MaxValue)]
    decimal? LimitPrice, // null if Market order
    [Range(1, double.MaxValue)]
    decimal Quantity
);