
dotnet tool install --global dotnet-ef
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet ef migrations add InitialCreate -o .\Data\Migrations
dotnet ef database update

%HOME%\AppData\Local\orderbook.db