namespace OrderBook.Data;

using Microsoft.EntityFrameworkCore;
using OrderBook.Models;

/// <summary>
/// Database context for the order book.
/// </summary>
public class OrderBookDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }

    public DbSet<Category> Categories { get; set; }

    public OrderBookDbContext(DbContextOptions<OrderBookDbContext> options)
            : base(options)
    {
    }
}