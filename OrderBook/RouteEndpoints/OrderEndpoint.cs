
using OrderBook.DTOs;
using OrderBook.Models;
using OrderBook.Services;

namespace OrderBook.RouteEndpoints;


public static class OrderEndpoint
{
    public static void SetupOrderEndpoints(this WebApplication app)
    {
        var groupOrder = app.MapGroup("order").WithParameterValidation();

        groupOrder.MapGet("/", GetOrders);
        groupOrder.MapGet("/{id}", GetOrderById).WithName(GetOrderByIdEndpointName);
        groupOrder.MapPost("/", PlaceOrder);
    }

    static async Task<IResult> GetOrders(IOrderService service)
    {
        var orders = await service.GetOrdersAsync();
        return Results.Ok(orders.Select(c =>
            new GetOrderResponse(c.Id, c.Type, c.Price, c.Quantity, c.CreatedAt, c.Category.Name)
        ));
    }

    static string GetOrderByIdEndpointName = "GetOrderById";
    static async Task<IResult> GetOrderById(int id, IOrderService service)
    {
        var order = await service.GetOrderAsync(id);

        if (order is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new GetOrderResponse(order.Id, order.Type, order.Price, order.Quantity, order.CreatedAt, order.Category.Name));
    }

    static async Task<IResult> PlaceOrder(PlaceOrderRequest request, IOrderService service, ICategoryService categoryService)
    {
        var category = await categoryService.GetCategoryByIdAsync(request.CategoryId);

        if (category is null)
        {
            return Results.NotFound();
        }

        try
        {
            var result = await service.PlaceOrderAsync(request.PlaceOrderType, request.LimitPrice, request.Quantity, category);

            // Exchange successful. Updated the orders in the market
            if (result.PlaceOrderResults.Count > 0)
            {
                return Results.Ok(new PlaceOrderResponse.UpdatedOrders(result.PlaceOrderResults));
            }

            // Unable to place the order, but the order request has created a new order.
            if (result.CreatedOrder is not null)
            {
                return Results.CreatedAtRoute(
                    GetOrderByIdEndpointName,
                    new { id = result.CreatedOrder.Id },
                    new PlaceOrderResponse.CreatedOrder(result.CreatedOrder, category.Name)
                );
            }

            // Unable to place the order, but the quantity of the order request is added to the existing order.
            if (result.UpdatedOrder is not null)
            {
                return Results.AcceptedAtRoute(
                    GetOrderByIdEndpointName,
                    new { id = result.UpdatedOrder.Id },
                    new PlaceOrderResponse.UpdatedOrder(result.UpdatedOrder, category.Name));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return Results.BadRequest();
    }
}