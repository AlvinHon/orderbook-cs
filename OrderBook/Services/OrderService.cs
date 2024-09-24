using Microsoft.EntityFrameworkCore;
using OrderBook.Data;
using OrderBook.Models;

namespace OrderBook.Services;

/// <summary>
/// Service implementing IOrderService for handling orders.
/// </summary>
public class OrderService : IOrderService
{
    private readonly OrderBookDbContext _dbCtx;

    public OrderService(OrderBookDbContext dbCtx)
    {
        _dbCtx = dbCtx;
    }

    public async Task<IOrderService.ServePlaceOrderResult> PlaceOrderAsync(PlaceOrderType type, decimal? limitPrice, decimal quantity, Category category)
    {
        var transaction = await _dbCtx.Database.BeginTransactionAsync();

        try
        {
            // Get the current orders avaliable in the market. Throws CannotDetermineMarketPriceException if the market price cannot be determined.
            var targetType = type == PlaceOrderType.Buy ? OrderType.Ask : OrderType.Bid;
            List<Order> ordersInMarket = await GetOrdersByTypeAsync(targetType, limitPrice, category);

            // Update the orders in the market.
            var results = ConsumeQuantityOfOrdersInMarket(ordersInMarket, quantity, category.Name);

            if (results.Count > 0)
            {
                // Update the orders in the database
                UpdateOrdersInMarketFromResults(results);

                await _dbCtx.SaveChangesAsync();
                await transaction.CommitAsync();
                return IOrderService.ServePlaceOrderResult.createPlaceOrderResult(results);
            }

            // If the quantity is not enough to fulfill the order, place the order in the market.
            var result = await HandlePlaceOrderDueToUnmatchQuantity(type, limitPrice, quantity, category);

            await _dbCtx.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Consume the quantity of the orders in the market.
    /// </summary>
    /// <param name="orders">The orders in the market.</param>
    /// <param name="quantity">The quantity to consume.</param>
    /// <param name="categoryName">The name of the category.</param>
    /// <returns>The results of the orders that have been updated.</returns>
    List<PlaceOrderResult> ConsumeQuantityOfOrdersInMarket(List<Order> orders, decimal quantity, string categoryName)
    {
        List<PlaceOrderResult> results = [];

        // Loop through the orders and check if the quantity is enough to fulfill the order
        orders.ForEach(order =>
        {
            if (quantity <= 0) return;

            if (order.Quantity >= quantity)
            {
                results.Add(new PlaceOrderResult(order, quantity, categoryName));
                quantity = 0;
            }
            else
            {
                quantity -= order.Quantity;

                results.Add(new PlaceOrderResult(order, order.Quantity, categoryName));
            }
        });

        return results;
    }

    /// <summary>
    /// Update the orders in the market from the results of the orders that have been placed.
    /// If the order has been fully consumed, delete the order.
    /// If the order has been partially consumed, update the quantity of the order.
    /// </summary>
    /// <param name="results">The results of the orders that have been placed.</param>
    void UpdateOrdersInMarketFromResults(List<PlaceOrderResult> results)
    {
        results.ForEach(async r =>
        {
            if (r.ConsumedQuantity == r.OriginalQuantity)
            {
                await DeleteOrderAsync(r.Id);
            }
            else if (r.ConsumedQuantity < r.OriginalQuantity)
            {
                var order = await GetOrderAsync(r.Id);
                order!.Quantity = r.OriginalQuantity - r.ConsumedQuantity;
            }
        });
    }

    /// <summary>
    /// Handle the place order due to the unmatch quantity. It is called when the quantity of the order
    /// cannot be consumed by the orders in the market.
    /// </summary>
    /// <param name="type">The type of the order.</param>
    /// <param name="limitPrice">The limit price of the order. Null if it is Market order.</param>
    /// <param name="quantity">The quantity of the order.</param>
    /// <param name="category">The category of the order.</param>
    /// <returns>The result of the order that has been placed.</returns>
    async Task<IOrderService.ServePlaceOrderResult> HandlePlaceOrderDueToUnmatchQuantity(PlaceOrderType type, decimal? limitPrice, decimal quantity, Category category)
    {
        // Get the orders in the market
        var targetType = type == PlaceOrderType.Buy ? OrderType.Bid : OrderType.Ask;
        List<Order> orders = await GetOrdersByTypeAsync(targetType, limitPrice, category);

        // if it is a limit order, the target price is the limit price, otherwise it is the market price which is 
        // the highest bid price or the lowest ask price, depending on the order type.
        var targetPrice =
            limitPrice ?? // Limit order
            targetType switch   // Market order
            {
                OrderType.Ask => orders.MinBy(o => o.Price)!.Price,
                OrderType.Bid => orders.MaxBy(o => o.Price)!.Price,
                _ => throw new NotImplementedException()
            };

        // find the order with same price
        var orderWithSamePrice = orders.Find(
                    o => o.Type == targetType &&
                    o.Price == limitPrice &&
                    o.CategoryId == category.Id);
        // update the quantity with same price
        if (orderWithSamePrice is not null)
        {
            orderWithSamePrice.Quantity += quantity;
            return IOrderService.ServePlaceOrderResult.createUpdatedOrderResult(orderWithSamePrice);
        }
        else // create a new order
        {
            return IOrderService.ServePlaceOrderResult.createCreatedOrderResult(
                await CreateOrderAsync(targetType, targetPrice, quantity, category)
            );
        }
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _dbCtx.Orders.Include(o => o.Category).FirstOrDefaultAsync(o => o.Id == id);
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _dbCtx.Orders.Include(o => o.Category).ToListAsync();
    }

    /// <summary>
    /// Get the orders by the type of the order, the limit price, and the category.
    /// The goal of this method is to get the orders that are available in the market.
    /// 
    /// There are two types of orders: Ask and Bid.
    /// The result contains the cheapest orders when the type is Ask and the most expensive orders when the type is Bid.
    /// If limit price is null, the result contains all orders.
    /// </summary>
    /// <param name="type">The type of the querying orders.</param>
    /// <param name="limitPrice">The limit price of the querying orders.</param>
    /// <param name="category">The category of the querying orders.</param>
    /// <returns>The list of orders.</returns>
    /// <exception cref="CannotDetermineMarketPriceException">Thrown when the market price cannot be determined.</exception>
    async Task<List<Order>> GetOrdersByTypeAsync(OrderType type, decimal? limitPrice, Category category)
    {
        var isMarketOrder = limitPrice is null;
        var isAsk = type == OrderType.Ask;

        var query = _dbCtx.Orders.Where(
            o => o.Type == type &&
            (isMarketOrder || (isAsk ? o.Price <= limitPrice : o.Price >= limitPrice)) &&
            o.CategoryId == category.Id);

        var results = isAsk ?
            await query.OrderBy(o => (double)o.Price).ToListAsync() :
            await query.OrderByDescending(o => (double)o.Price).ToListAsync();

        // If the limit price is null and there are no orders available in the market, throw an exception
        if (isMarketOrder && results.Count == 0)
        {
            throw new CannotDetermineMarketPriceException();
        }

        return results;
    }

    async Task<Order> CreateOrderAsync(OrderType type, decimal price, decimal quantity, Category category)
    {
        var order = new Order
        {
            Type = type,
            Price = price,
            Quantity = quantity,
            CategoryId = category.Id,
            CreatedAt = DateTime.Now,
            Category = category
        };

        await _dbCtx.Orders.AddAsync(order);
        return order;
    }

    async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _dbCtx.Orders.FindAsync(id);

        if (order is null) return false;

        _dbCtx.Orders.Remove(order);
        await _dbCtx.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Exception thrown when the market price cannot be determined.
    /// </summary>
    public class CannotDetermineMarketPriceException : Exception
    {
        public CannotDetermineMarketPriceException() : base("Cannot determine market price")
        {
        }
    }
}