
using OrderBook.DTOs;
using OrderBook.Services;

namespace OrderBook.RouteEndpoints;


public static class OrderEndpoint
{
    public static void SetupOrderEndpoints(this WebApplication app)
    {
        var groupOrder = app.MapGroup("order").WithParameterValidation();

        groupOrder.MapGet("/", GetOrders);
        groupOrder.MapGet("/{id}", GetOrderById);
        groupOrder.MapPost("/", PlaceOrder);
        groupOrder.MapDelete("/{id}", DeleteOrder);
    }


    static async Task<IResult> GetOrders(IOrderService service)
    {
        var orders = await service.GetOrdersAsync();
        return Results.Ok(orders.Select(c => new GetOrderResponse(c.Id, c.Type, c.Price, c.CreatedAt, c.Category.Name)));
    }

    static async Task<IResult> GetOrderById(int id, IOrderService service)
    {
        var order = await service.GetOrderAsync(id);

        if (order is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new GetOrderResponse(order.Id, order.Type, order.Price, order.CreatedAt, order.Category.Name));
    }

    static async Task<IResult> PlaceOrder(PlaceOrderRequest request, IOrderService service, ICategoryService categoryService)
    {
        var category = await categoryService.GetCategoryByIdAsync(request.CategoryId);

        if (category is null)
        {
            return Results.NotFound();
        }

        var placeOrders = await service.PlaceOrderAsync(request.PlaceOrderType, request.LimitPrice, request.Quantity, category);

        return Results.Ok(new PlaceOrderResponse(placeOrders));

    }

    static async Task<IResult> DeleteOrder(int id, IOrderService service)
    {
        var result = await service.DeleteOrderAsync(id);

        if (!result)
        {
            return Results.NotFound();
        }

        return Results.NoContent();
    }
}