using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace OrderBook.DTOs;

/// <summary>
/// Request to create a category.
/// </summary>
/// <param name="Name">The name of the category.</param>
public record CreateCategoryRequest(
    [Required, NotNull] string Name
);