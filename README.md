# Order Book API Server

It is an example project for an API server implemented in `ASP.NET C#` that allows users to place order to order books.

The web application implements a simple logic of order book:
- An `Order` is consisted of an amount of Price and a quantity.
- Order can be placed for an action either a `Buy` or a `Sell`.
- Placing a Buy order will consume the quantity of the selling order (i.e. Ask) in the market with lowest price first.
- Placing a Sell order will consume the quantity of the buying order (i.e. Bid) in the market with highest price first. 
- If the market cannot filfull the Buy/Sell request, either a new order will be place in the market or existing order with same price will increase the quantity.
- Orders are grouped by `Category`.

## API

Overview of the supported APIs
- GET /category
- GET /category/{id}
- POST /category
- PUT /category
- Delete /category/{id}
- GET /order/
- GET /order/{id}
- POST /order

## Initialize Database

```sh
cd OrderBook
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate -o .\Data\Migrations
dotnet ef database update
```

A local sqlite database named `orderbook.db` will be create at `\Order\orderbook.db`

## Build & Run

```sh
cd OrderBook
dotnet build
dotnet run
```

The web application will run at http://localhost:5022.

For development, run `dotnet watch` so that the web application gets instant reflection upon source code modification.