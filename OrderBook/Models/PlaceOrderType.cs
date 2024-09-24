using System.Text.Json.Serialization;

namespace OrderBook.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PlaceOrderType
{
    Buy,
    Sell,
}