using OrderBook.Models;

namespace OrderBook.Services;

/// <summary>
/// Interface for the service for the category management.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Get all categories.
    /// </summary>
    /// <returns>The list of categories.</returns>
    Task<List<Category>> GetCategoriesAsync();

    Task<Category?> GetCategoryByIdAsync(int id);

    Task<Category> CreateCategoryAsync(string name);

    Task<Category?> UpdateCategoryAsync(int id, string name);

    Task<bool> DeleteCategoryAsync(int id);
}