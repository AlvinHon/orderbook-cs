using OrderBook.DTOs;
using OrderBook.Models;
using OrderBook.Services;

namespace OrderBook.RouteEndpoints;

public static class CategoryEndpoints
{
    public static void SetupCategoryEndpoints(this WebApplication app)
    {
        var groupCategory = app.MapGroup("category").WithParameterValidation();

        groupCategory.MapGet("/", GetCategories);
        groupCategory.MapGet("/{id}", GetCategoryById).WithName(GetCategoryByIdEndpointName);
        groupCategory.MapPost("/", CreateCategory);
        groupCategory.MapPut("/{id}", UpdateCategory);
        groupCategory.MapDelete("/{id}", DeleteCategory);
    }

    static async Task<IResult> GetCategories(ICategoryService service)
    {
        var categories = await service.GetCategoriesAsync();
        return Results.Ok(categories.Select(c => new GetCategoryResult(c.Id, c.Name)));
    }

    static string GetCategoryByIdEndpointName = "GetCategoryById";

    static async Task<IResult> GetCategoryById(int id, ICategoryService service)
    {
        Category? category = await service.GetCategoryByIdAsync(id);

        return category switch
        {
            null => Results.NotFound(),
            _ => Results.Ok(new GetCategoryResult(category.Id, category.Name))
        };
    }

    static async Task<IResult> CreateCategory(CreateCategoryRequest request, ICategoryService service)
    {
        Category category = await service.CreateCategoryAsync(request.Name);

        return Results.CreatedAtRoute(
            GetCategoryByIdEndpointName,
            new { id = category.Id },
            new GetCategoryResult(category.Id, category.Name));
    }

    static async Task<IResult> UpdateCategory(int id, UpdateCategoryRequest request, ICategoryService service)
    {
        Category? category = await service.UpdateCategoryAsync(id, request.Name);

        return category switch
        {
            null => Results.NotFound(),
            _ => Results.NoContent()
        };
    }


    static async Task<IResult> DeleteCategory(int id, ICategoryService service)
    {
        return await service.DeleteCategoryAsync(id) ? Results.NoContent() : Results.NotFound();
    }
}