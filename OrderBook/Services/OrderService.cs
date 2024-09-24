using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyModel;
using OrderBook.Data;
using OrderBook.Models;

namespace OrderBook.Services;


public class OrderService : IOrderService
{
    private readonly OrderBookDbContext _dbCtx;

    public OrderService(OrderBookDbContext dbCtx)
    {
        _dbCtx = dbCtx;
    }

    public async Task<List<PlaceOrderResult>> PlaceOrderAsync(PlaceOrderType type, decimal? limitPrice, decimal quantity, Category category)
    {
        var transaction = await _dbCtx.Database.BeginTransactionAsync();

        List<Order> ordersInMarket = [];
        try
        {
            Console.WriteLine($"Placing order: {type} {limitPrice} {quantity} {category.Name}");
            // Get the current orders avaliable in the market
            var targetOrderType = type == PlaceOrderType.Buy ? OrderType.Ask : OrderType.Bid;
            ordersInMarket = targetOrderType switch
            {
                OrderType.Ask => await GetAskOrdersByLimitPriceAsync(limitPrice, category),
                OrderType.Bid => await GetBidOrdersByLimitPriceAsync(limitPrice, category),
                _ => throw new NotImplementedException()
            };

            Console.WriteLine($"Orders in market: {ordersInMarket.Count}");

            // If the limit price is null and there are no orders available in the market, throw an exception
            if (limitPrice is null && ordersInMarket.Count == 0)
            {
                throw new CannotDetermineMarketPriceException();
            }

            // Update the orders in the market. Throws InsufficientQuantityException if the market quantity is not enough
            var results = ConsumeQuantityOfOrdersInMarket(ordersInMarket, quantity);

            // If the quantity is still greater than 0, handle the order due to unmatched quantity
            if (results.Count == 0)
            {
                await HandlePlaceOrderDueToUnmatchQuantity(ordersInMarket, targetOrderType, limitPrice, quantity, category);
            }
            else
            {
                // Update the orders in the database
                UpdateOrdersInMarketFromResults(results);
            }

            await _dbCtx.SaveChangesAsync();

            return results;

        }
        catch (InsufficientQuantityException)
        {
            await transaction.RollbackAsync();
            return [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            await transaction.RollbackAsync();
            return [];
        }
    }

    List<PlaceOrderResult> ConsumeQuantityOfOrdersInMarket(List<Order> orders, decimal quantity)
    {
        List<PlaceOrderResult> results = [];

        // Loop through the orders and check if the quantity is enough to fulfill the order
        orders.ForEach(order =>
        {
            if (quantity <= 0) return;

            if (order.Quantity >= quantity)
            {
                results.Add(PlaceOrderResult.consumeQuantity(ref order, quantity));
                quantity = 0;
            }
            else
            {
                quantity -= order.Quantity;

                results.Add(PlaceOrderResult.consumeQuantity(ref order, order.Quantity));
            }
        });

        // If the quantity is still greater than 0, return empty list
        return quantity > 0 ? [] : results;
    }

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


    async Task HandlePlaceOrderDueToUnmatchQuantity(List<Order> orders, OrderType type, decimal? limitPrice, decimal quantity, Category category)
    {
        // if it is a limit order, the target price is the limit price, otherwise it is the market price which is 
        // the highest bid price or the lowest ask price, depending on the order type.
        var targetPrice =
            limitPrice ?? // Limit order
            type switch   // Market order
            {
                OrderType.Ask => orders.MinBy(o => o.Price)!.Price,
                OrderType.Bid => orders.MaxBy(o => o.Price)!.Price,
                _ => throw new NotImplementedException()
            };

        // find the order with same price
        var orderWithSamePrice = orders.First(
                    o => o.Type == type &&
                    o.Price == limitPrice &&
                    o.CategoryId == category.Id);

        // update the quantity with same price
        if (orderWithSamePrice is not null)
        {
            orderWithSamePrice.Quantity += quantity;
        }
        else // create a new order
        {
            await CreateOrderAsync(type, targetPrice, quantity, category);
        }
    }

    public async Task<Order?> GetOrderAsync(int id)
    {
        return await _dbCtx.Orders.FindAsync(id);
    }

    public async Task<List<Order>> GetOrdersAsync()
    {
        return await _dbCtx.Orders.ToListAsync();
    }

    async Task<List<Order>> GetAskOrdersByLimitPriceAsync(decimal? limitPrice, Category category)
    {
        var isMarketOrder = limitPrice is null;
        return await _dbCtx.Orders.Where(
                o => o.Type == OrderType.Ask &&
                (isMarketOrder || o.Price <= limitPrice) &&
                o.CategoryId == category.Id)
                .OrderBy(o => o.Price)
                .ToListAsync();
    }

    async Task<List<Order>> GetBidOrdersByLimitPriceAsync(decimal? limitPrice, Category category)
    {
        var isMarketOrder = limitPrice is null;
        return await _dbCtx.Orders.Where(
                o => o.Type == OrderType.Bid &&
                (isMarketOrder || o.Price >= limitPrice) &&
                o.CategoryId == category.Id)
                .OrderByDescending(o => o.Price)
                .ToListAsync();
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

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _dbCtx.Orders.FindAsync(id);

        if (order is null) return false;

        _dbCtx.Orders.Remove(order);
        await _dbCtx.SaveChangesAsync();
        return true;
    }

    class InsufficientQuantityException : Exception
    {
        public InsufficientQuantityException() : base("Insufficient quantity")
        {
        }
    }

    class CannotDetermineMarketPriceException : Exception
    {
        public CannotDetermineMarketPriceException() : base("Cannot determine market price")
        {
        }
    }
}