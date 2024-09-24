using System.Text.Json.Serialization;

namespace OrderBook.Models;

/// <summary>
/// Represents the type of a User action to place an order.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaceOrderType
{
    Buy,
    Sell,
}