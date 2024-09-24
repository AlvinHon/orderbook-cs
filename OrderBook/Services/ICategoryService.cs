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

    /// <summary>
    /// Get a category by its id.
    /// </summary>
    /// <param name="id">The id of the querying category.</param>
    /// <returns>The category with the given id, or null if not found.</returns>
    Task<Category?> GetCategoryByIdAsync(int id);

    /// <summary>
    /// Create a category.
    /// </summary>
    /// <param name="name">The name of the category.</param>
    /// <returns>The created category.</returns>
    Task<Category> CreateCategoryAsync(string name);

    /// <summary>
    /// Update a category.
    /// </summary>
    /// <param name="id">The id of the category to update.</param>
    /// <param name="name">The new name of the category.</param>
    /// <returns>The updated category, or null if the category does not exist.</returns>
    Task<Category?> UpdateCategoryAsync(int id, string name);

    /// <summary>
    /// Delete a category.
    /// </summary>
    /// <param name="id">The id of the category to delete.</param>
    /// <returns>True if the category is deleted, false if the category does not exist.</returns>
    Task<bool> DeleteCategoryAsync(int id);
}