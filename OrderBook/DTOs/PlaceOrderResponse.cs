
using OrderBook.Models;

namespace OrderBook.DTOs;

public record PlaceOrderResponse(List<PlaceOrderResult> PlaceOrderResults);