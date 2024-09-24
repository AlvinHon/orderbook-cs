using OrderBook.Models;

namespace OrderBook.Services;

public interface IOrderService
{
    /// <summary>
    /// Get all orders in the market.
    /// </summary>
    /// <returns>The list of orders in the market.</returns>
    Task<List<Order>> GetOrdersAsync();

    /// <summary>
    /// Get an order by its id.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>The order with the given id, or null if not found.</returns>
    Task<Order?> GetOrderAsync(int id);

    /// <summary>
    /// Place an order in the market.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="limitPrice"></param>
    /// <param name="quantity"></param>
    /// <param name="category"></param>
    /// <returns>Either the list of the updated orders, the created order or updated order.</returns>
    /// <exception cref="CannotDetermineMarketPriceException"></exception>
    Task<ServePlaceOrderResult> PlaceOrderAsync(PlaceOrderType type, decimal? limitPrice, decimal quantity, Category category);

    /// <summary>
    /// The return type of the PlaceOrderAsync method.
    /// There are three possible results: the list of the updated orders, the created order, or the updated order.
    /// </summary>
    public class ServePlaceOrderResult
    {
        public List<PlaceOrderResult> PlaceOrderResults { get; private set; } = new();
        public Order? CreatedOrder { get; private set; }
        public Order? UpdatedOrder { get; private set; }

        private ServePlaceOrderResult() { }

        public static ServePlaceOrderResult createPlaceOrderResult(List<PlaceOrderResult> placeOrderResults)
        {
            return new ServePlaceOrderResult
            {
                PlaceOrderResults = placeOrderResults
            };
        }

        public static ServePlaceOrderResult createCreatedOrderResult(Order createdOrder)
        {
            return new ServePlaceOrderResult
            {
                CreatedOrder = createdOrder
            };
        }

        public static ServePlaceOrderResult createUpdatedOrderResult(Order updatedOrder)
        {
            return new ServePlaceOrderResult
            {
                UpdatedOrder = updatedOrder
            };
        }
    }
}