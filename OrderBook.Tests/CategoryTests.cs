namespace OrderBook.Tests;

using OrderBook.Services;

public class CategoryTests
{
    [Fact]
    async public void CreateCategoryAsync()
    {
        using var db = new TestDB();

        CategoryService service = new CategoryService(db.Context);

        // Check if there are no categories at the beginning
        var categories = await service.GetCategoriesAsync();
        Assert.Empty(categories);

        // Create a new category and check if it was created with the correct values
        var createdCategory = await service.CreateCategoryAsync("Category 1");
        Assert.NotNull(createdCategory);
        Assert.Equal(1, createdCategory.Id);
        Assert.Equal("Category 1", createdCategory.Name);

        // Check if the category was added to the database
        var categories2 = await service.GetCategoriesAsync();
        Assert.Single(categories2);
    }

    [Fact]
    async public void UpdateCategoryAsync()
    {
        using var db = new TestDB();

        CategoryService service = new CategoryService(db.Context);

        // Create a new category
        var createdCategory = await service.CreateCategoryAsync("Category 1");

        // Update the category and check if it was updated with the correct values
        var updatedCategory = await service.UpdateCategoryAsync(createdCategory.Id, "Category 2");
        Assert.NotNull(updatedCategory);
        Assert.Equal(1, updatedCategory.Id);
        Assert.Equal("Category 2", updatedCategory.Name);
    }


    [Fact]
    async public void DeleteCategoryAsync()
    {
        using var db = new TestDB();

        CategoryService service = new CategoryService(db.Context);

        // Create a new category
        var createdCategory = await service.CreateCategoryAsync("Category 1");

        // Check if the category was added to the database
        var categories = await service.GetCategoriesAsync();
        Assert.Single(categories);

        // Delete the category and check if it was deleted
        var deleted = await service.DeleteCategoryAsync(createdCategory.Id);
        Assert.True(deleted);

        // Check if the category was removed from the database
        var categories2 = await service.GetCategoriesAsync();
        Assert.Empty(categories2);
    }
}