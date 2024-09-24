using System.ComponentModel.DataAnnotations;

namespace OrderBook.DTOs;

public record UpdateCategoryRequest(
    [Required] string Name
);

