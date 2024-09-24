using OrderBook.Models;

namespace OrderBook.Services;

public interface IOrderService
{
    Task<List<Order>> GetOrdersAsync();
    Task<Order?> GetOrderAsync(int id);
    Task<List<PlaceOrderResult>> PlaceOrderAsync(PlaceOrderType type, decimal? limitPrice, decimal quantity, Category category);
    Task<bool> DeleteOrderAsync(int id);
}