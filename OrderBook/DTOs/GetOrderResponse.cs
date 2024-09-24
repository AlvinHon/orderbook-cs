using OrderBook.Models;

namespace OrderBook.DTOs;

public record GetOrderResponse(int Id, OrderType type, decimal Price, DateTime CreatedAt, string Category);