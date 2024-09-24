using System.Text.Json.Serialization;

namespace OrderBook.Models;

/// <summary>
/// Represents the type of an order in Database domain.
/// Ask is a sell order while Bid is a buy order.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum OrderType
{
    Ask,
    Bid,
}