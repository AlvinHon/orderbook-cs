using Microsoft.EntityFrameworkCore;
using OrderBook.Data;
using OrderBook.RouteEndpoints;
using OrderBook.Services;

// Build application with database and services
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<OrderBookDbContext>((options) =>
    options.UseSqlite("Data Source=orderbook.db")
);
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
var app = builder.Build();

// Setup Endpoints
app.MapGet("/", () => "Order Book API Server!");
app.SetupCategoryEndpoints();
app.SetupOrderEndpoints();

// Run the application
app.Run();
