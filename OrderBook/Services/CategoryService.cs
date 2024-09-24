using Microsoft.EntityFrameworkCore;
using OrderBook.Data;
using OrderBook.Models;

namespace OrderBook.Services;

/// <summary>
/// Service implementing CRUD operations for Category.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly OrderBookDbContext _dbCtx;
    public CategoryService(OrderBookDbContext dbCtx)
    {
        _dbCtx = dbCtx;
    }

    async public Task<List<Category>> GetCategoriesAsync()
    {
        return await _dbCtx.Categories.ToListAsync();
    }

    async public Task<Category> CreateCategoryAsync(string name)
    {
        Category category = new()
        {
            Name = name
        };
        await _dbCtx.Categories.AddAsync(category);
        await _dbCtx.SaveChangesAsync();
        return category;
    }

    async public Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _dbCtx.Categories.FindAsync(id);
    }

    async public Task<Category?> UpdateCategoryAsync(int id, string name)
    {
        Category? category = await _dbCtx.Categories.FindAsync(id);
        if (category is not null)
        {
            category.Name = name;
            await _dbCtx.SaveChangesAsync();
        }
        return category;
    }

    async public Task<bool> DeleteCategoryAsync(int id)
    {
        Category? category = await _dbCtx.Categories.FindAsync(id);

        if (category is null) return false;

        _dbCtx.Categories.Remove(category);
        await _dbCtx.SaveChangesAsync();
        return true;
    }

}