namespace OrderBook.DTOs;

/// <summary>
/// Represents the response of getting an order.
/// </summary>
/// <param name="Id">The id of the order.</param>
/// <param name="Name">The name of the category of the order.</param>
public record GetCategoryResult(int Id, string Name);