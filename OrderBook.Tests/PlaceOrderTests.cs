namespace OrderBook.Tests;

using OrderBook.DTOs;
using OrderBook.Models;
using OrderBook.Services;

public class PlaceOrderTests
{

    [Fact]
    async public void PlaceLimitOrderBuy()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // 
        // Order: Buy 100 at limit price 100
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 100   | 100      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);

        OrderService orderService = new(db.Context);

        // Check if there are no orders at the beginning
        var orders = await orderService.GetOrdersAsync();
        Assert.Empty(orders);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Buy, 100, 100, category);
        Assert.NotNull(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Empty(result.PlaceOrderResults);

        Assert.Equal(1, result.CreatedOrder.Id);
        Assert.Equal(OrderType.Bid, result.CreatedOrder.Type);
        Assert.Equal(100, result.CreatedOrder.Price);
        Assert.Equal(100, result.CreatedOrder.Quantity);
        Assert.Equal(category.Id, result.CreatedOrder.CategoryId);

        // Check if the order was added to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        var order = orders2[0];
        Assert.Equal(1, order.Id);
        Assert.Equal(OrderType.Bid, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(100, order.Quantity);
        Assert.Equal(category.Id, order.CategoryId);
    }


    [Fact]
    async public void PlaceLimitOrderBuyWithSamePrice()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 100   | 100      |
        // 
        // Order: Buy 100 at limit price 100
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 100   | 200      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Bid, 100, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Single(orders);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Buy, 100, 100, category);
        Assert.Null(result.CreatedOrder);
        Assert.NotNull(result.UpdatedOrder);
        Assert.Empty(result.PlaceOrderResults);

        Assert.Equal(1, result.UpdatedOrder.Id);
        Assert.Equal(OrderType.Bid, result.UpdatedOrder.Type);
        Assert.Equal(100, result.UpdatedOrder.Price);
        Assert.Equal(200, result.UpdatedOrder.Quantity);
        Assert.Equal(category.Id, result.UpdatedOrder.CategoryId);

        // Check if the order was updated to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        var order = orders2[0];
        Assert.Equal(1, order.Id);
        Assert.Equal(OrderType.Bid, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(200, order.Quantity);
        Assert.Equal(category.Id, order.CategoryId);
    }


    [Fact]
    async public void PlaceLimitOrderSell()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // 
        // Order: Sell 100 at limit price 100
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100   | 100      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);

        OrderService orderService = new(db.Context);

        // Check if there are no orders at the beginning
        var orders = await orderService.GetOrdersAsync();
        Assert.Empty(orders);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Sell, 100, 100, category);
        Assert.NotNull(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Empty(result.PlaceOrderResults);

        Assert.Equal(1, result.CreatedOrder.Id);
        Assert.Equal(OrderType.Ask, result.CreatedOrder.Type);
        Assert.Equal(100, result.CreatedOrder.Price);
        Assert.Equal(100, result.CreatedOrder.Quantity);
        Assert.Equal(category.Id, result.CreatedOrder.CategoryId);

        // Check if the order was added to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        var order = orders2[0];
        Assert.Equal(1, order.Id);
        Assert.Equal(OrderType.Ask, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(100, order.Quantity);
        Assert.Equal(category.Id, order.CategoryId);
    }

    [Fact]
    async public void PlaceLimitOrderSellWithSamePrice()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100   | 100      |
        // 
        // Order: Sell 100 at limit price 100
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100   | 200      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Ask, 100, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Single(orders);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Sell, 100, 100, category);
        Assert.Null(result.CreatedOrder);
        Assert.NotNull(result.UpdatedOrder);
        Assert.Empty(result.PlaceOrderResults);

        Assert.Equal(1, result.UpdatedOrder.Id);
        Assert.Equal(OrderType.Ask, result.UpdatedOrder.Type);
        Assert.Equal(100, result.UpdatedOrder.Price);
        Assert.Equal(200, result.UpdatedOrder.Quantity);
        Assert.Equal(category.Id, result.UpdatedOrder.CategoryId);

        // Check if the order was updated to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        var order = orders2[0];
        Assert.Equal(1, order.Id);
        Assert.Equal(OrderType.Ask, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(200, order.Quantity);
        Assert.Equal(category.Id, order.CategoryId);
    }

    [Fact]
    async public void PlaceLimitOrderBuy_ShouldConsumeQuantityInMarket()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100   | 100      |
        // | Ask  | 50    | 100      |
        // 
        // Order: Buy 150 at limit price 100
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100    | 50      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Ask, 50, 100, category);
        AddOrder(db, OrderType.Ask, 100, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Equal(2, orders.Count);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Buy, 100, 150, category);
        Assert.Null(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Equal(2, result.PlaceOrderResults.Count); // the Bid order that was consumed

        // The result of the consumed order
        result.PlaceOrderResults.Sort((a, b) => a.Id.CompareTo(b.Id));
        var placeOrderResult = result.PlaceOrderResults[0];
        Assert.Equal(1, placeOrderResult.Id);
        Assert.Equal(OrderType.Ask, placeOrderResult.Type);
        Assert.Equal(50, placeOrderResult.PriceAt);
        Assert.Equal(100, placeOrderResult.OriginalQuantity);
        Assert.Equal(100, placeOrderResult.ConsumedQuantity); // the whole order was consumed
        Assert.Equal(category.Name, placeOrderResult.Category);

        var placeOrderResult2 = result.PlaceOrderResults[1];
        Assert.Equal(2, placeOrderResult2.Id);
        Assert.Equal(OrderType.Ask, placeOrderResult2.Type);
        Assert.Equal(100, placeOrderResult2.PriceAt);
        Assert.Equal(100, placeOrderResult2.OriginalQuantity);
        Assert.Equal(50, placeOrderResult2.ConsumedQuantity); // the whole order was consumed partially
        Assert.Equal(category.Name, placeOrderResult2.Category);

        // Check if the ask order was delete to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        // The remaining ask order
        var order = orders2[0];
        Assert.Equal(2, order.Id);
        Assert.Equal(OrderType.Ask, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(50, order.Quantity);
    }

    [Fact]
    async public void PlaceLimitOrderBuy_ShouldConsumeLimitedQuantityInMarket()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100   | 100      |
        // | Ask  | 50    | 100      |
        // 
        // Order: Buy 150 at limit price 50
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100    | 100     |
        //
        // (The result of consumed order should be only 100 at price 50)


        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Ask, 50, 100, category);
        AddOrder(db, OrderType.Ask, 100, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Equal(2, orders.Count);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Buy, 50, 150, category);
        Assert.Null(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Single(result.PlaceOrderResults); // the Bid order that was consumed

        // The result of the consumed order
        var placeOrderResult = result.PlaceOrderResults[0];
        Assert.Equal(1, placeOrderResult.Id);
        Assert.Equal(OrderType.Ask, placeOrderResult.Type);
        Assert.Equal(50, placeOrderResult.PriceAt);
        Assert.Equal(100, placeOrderResult.OriginalQuantity);
        Assert.Equal(100, placeOrderResult.ConsumedQuantity); // the whole order was consumed
        Assert.Equal(category.Name, placeOrderResult.Category);

        // Check if the ask order was delete to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        // The remaining ask order
        var order = orders2[0];
        Assert.Equal(2, order.Id);
        Assert.Equal(OrderType.Ask, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(100, order.Quantity);
    }

    [Fact]
    async public void PlaceLimitOrderSell_ShouldConsumeQuantityInMarket()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 200   | 100      |
        // | Bid  | 100   | 100      |
        // 
        // Order: Sell 150 at limit price 100
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 100   | 50      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Bid, 200, 100, category);
        AddOrder(db, OrderType.Bid, 100, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Equal(2, orders.Count);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Sell, 100, 150, category);
        Assert.Null(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Equal(2, result.PlaceOrderResults.Count); // the Bid order that was consumed

        // The result of the consumed order
        result.PlaceOrderResults.Sort((a, b) => a.Id.CompareTo(b.Id));
        var placeOrderResult = result.PlaceOrderResults[0];
        Assert.Equal(1, placeOrderResult.Id);
        Assert.Equal(OrderType.Bid, placeOrderResult.Type);
        Assert.Equal(200, placeOrderResult.PriceAt);
        Assert.Equal(100, placeOrderResult.OriginalQuantity);
        Assert.Equal(100, placeOrderResult.ConsumedQuantity); // the whole order was consumed
        Assert.Equal(category.Name, placeOrderResult.Category);

        var placeOrderResult2 = result.PlaceOrderResults[1];
        Assert.Equal(2, placeOrderResult2.Id);
        Assert.Equal(OrderType.Bid, placeOrderResult2.Type);
        Assert.Equal(100, placeOrderResult2.PriceAt);
        Assert.Equal(100, placeOrderResult2.OriginalQuantity);
        Assert.Equal(50, placeOrderResult2.ConsumedQuantity); // the whole order was consumed partially
        Assert.Equal(category.Name, placeOrderResult2.Category);

        // Check if the ask order was delete to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        // The remaining ask order
        var order = orders2[0];
        Assert.Equal(2, order.Id);
        Assert.Equal(OrderType.Bid, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(50, order.Quantity);
    }

    [Fact]
    async public void PlaceLimitOrderSell_ShouldConsumeLimitedQuantityInMarket()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 200   | 100      |
        // | Bid  | 100   | 100      |
        // 
        // Order: Sell 150 at limit price 150
        // 
        // Expected:
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 100    | 100     |
        //
        // (The result of consumed order should be only 100 at price 200)


        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Bid, 200, 100, category);
        AddOrder(db, OrderType.Bid, 100, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Equal(2, orders.Count);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Sell, 150, 150, category);
        Assert.Null(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Single(result.PlaceOrderResults); // the Bid order that was consumed

        // The result of the consumed order
        var placeOrderResult = result.PlaceOrderResults[0];
        Assert.Equal(1, placeOrderResult.Id);
        Assert.Equal(OrderType.Bid, placeOrderResult.Type);
        Assert.Equal(200, placeOrderResult.PriceAt);
        Assert.Equal(100, placeOrderResult.OriginalQuantity);
        Assert.Equal(100, placeOrderResult.ConsumedQuantity); // the whole order was consumed
        Assert.Equal(category.Name, placeOrderResult.Category);

        // Check if the ask order was delete to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        // The remaining ask order
        var order = orders2[0];
        Assert.Equal(2, order.Id);
        Assert.Equal(OrderType.Bid, order.Type);
        Assert.Equal(100, order.Price);
        Assert.Equal(100, order.Quantity);
    }

    [Fact]
    async public void PlaceMarketOrderBuy_ShouldConsumeQuantityInMarket()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 200   | 100      |
        // | Ask  | 100   | 100      |
        // 
        // Order: Buy 100 at market price (=100)
        // 
        // Expected: (deleted ask order)
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Ask  | 200   | 100      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Ask, 100, 100, category);
        AddOrder(db, OrderType.Ask, 200, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Equal(2, orders.Count);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Buy, null, 100, category);
        Assert.Null(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Single(result.PlaceOrderResults); // the ask order that was consumed

        // The result of the consumed order
        var placeOrderResult = result.PlaceOrderResults[0];
        Assert.Equal(1, placeOrderResult.Id);
        Assert.Equal(OrderType.Ask, placeOrderResult.Type);
        Assert.Equal(100, placeOrderResult.PriceAt);
        Assert.Equal(100, placeOrderResult.OriginalQuantity);
        Assert.Equal(100, placeOrderResult.ConsumedQuantity); // the whole order was consumed
        Assert.Equal(category.Name, placeOrderResult.Category);

        // Check if the ask order was delete to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        // The remaining ask order
        var order = orders2[0];
        Assert.Equal(OrderType.Ask, order.Type);
        Assert.Equal(200, order.Price);
    }


    [Fact]
    async public void PlaceMarketOrderSell_ShouldConsumeQuantityInMarket()
    {
        // Market: 
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 100   | 100      |
        // | Bid  | 50    | 100      |
        // 
        // Order: Sell 100 at market price (=100)
        // 
        // Expected: (deleted bid order)
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // | Bid  | 50    | 100      |

        using var db = new TestDB();
        var category = await AddTestCategory(db);
        AddOrder(db, OrderType.Bid, 100, 100, category);
        AddOrder(db, OrderType.Bid, 50, 100, category);

        OrderService orderService = new(db.Context);

        // Check the initial orders
        var orders = await orderService.GetOrdersAsync();
        Assert.Equal(2, orders.Count);

        // Place an order and check if it was created with the correct values
        var result = await orderService.PlaceOrderAsync(PlaceOrderType.Sell, null, 100, category);
        Assert.Null(result.CreatedOrder);
        Assert.Null(result.UpdatedOrder);
        Assert.Single(result.PlaceOrderResults); // the Bid order that was consumed

        // The result of the consumed order
        var placeOrderResult = result.PlaceOrderResults[0];
        Assert.Equal(1, placeOrderResult.Id);
        Assert.Equal(OrderType.Bid, placeOrderResult.Type);
        Assert.Equal(100, placeOrderResult.PriceAt);
        Assert.Equal(100, placeOrderResult.OriginalQuantity);
        Assert.Equal(100, placeOrderResult.ConsumedQuantity); // the whole order was consumed
        Assert.Equal(category.Name, placeOrderResult.Category);

        // Check if the ask order was delete to the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Single(orders2);

        // The remaining ask order
        var order = orders2[0];
        Assert.Equal(OrderType.Bid, order.Type);
        Assert.Equal(50, order.Price);
    }

    [Fact]
    async public void PlaceMarketOrder_ShouldThrowException_whenNoOrdersInMarket()
    {
        // Market: (no orders)
        // | Type | Price | Quantity |
        // |------|-------|----------|
        // 
        // Order: Buy 100 at market price
        // 
        // Expected: throw exception

        using var db = new TestDB();
        var category = await AddTestCategory(db);

        OrderService orderService = new(db.Context);

        // Check if there are no orders at the beginning
        var orders = await orderService.GetOrdersAsync();
        Assert.Empty(orders);

        // Place an order and check if it was created with the correct values
        Func<Task> action = () => orderService.PlaceOrderAsync(PlaceOrderType.Buy, null, 100, category);
        var exception = await Assert.ThrowsAsync<OrderService.CannotDetermineMarketPriceException>(action);
        Assert.Equal("Cannot determine market price", exception.Message);

        // Check if there are no orders in the database
        var orders2 = await orderService.GetOrdersAsync();
        Assert.Empty(orders2);
    }

    // Helper functions


    async Task<Category> AddTestCategory(TestDB db)
    {
        CategoryService categoryService = new(db.Context);
        return await categoryService.CreateCategoryAsync("Category 2");
    }

    async void AddOrder(TestDB db, OrderType type, decimal price, decimal quantity, Category category)
    {
        await db.Context.Orders.AddAsync(new Order
        {
            Type = type,
            Price = price,
            Quantity = quantity,
            CategoryId = category.Id,
            Category = category
        });

        await db.Context.SaveChangesAsync();
    }
}
