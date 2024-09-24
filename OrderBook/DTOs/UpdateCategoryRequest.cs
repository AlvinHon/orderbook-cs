using System.ComponentModel.DataAnnotations;

namespace OrderBook.DTOs;

/// <summary>
/// Represents the request of updating a category.
/// </summary>
/// <param name="Name">The name of the category.</param>
public record UpdateCategoryRequest(
    [Required] string Name
);

