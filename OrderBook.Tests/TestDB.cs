
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using OrderBook.Data;

class TestDB : IDisposable
{
    private string dbName;
    public OrderBookDbContext Context { get; private set; }

    public TestDB()
    {
        dbName = "OrderBook" + Guid.NewGuid().ToString();
        var builder = new DbContextOptionsBuilder<OrderBookDbContext>();
        builder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        var options = builder.UseInMemoryDatabase(dbName).Options;

        Context = new OrderBookDbContext(options);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
    }
}