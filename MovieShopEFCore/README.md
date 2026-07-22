# MovieShop EF Core

MovieShop is a learning project that connects an ASP.NET Core MVC website to a
SQL Server database through an ASP.NET Core Web API and Entity Framework Core.

## Projects

- `MovieShop.Api` — REST API, EF Core entities, `DbContext`, and migrations.
- `MovieShop.Web` — MVC user interface that reads movie data from the API.

## Prerequisites

- .NET 10 SDK
- SQL Server (a local installation or Docker container)

## Local setup

1. Restore the projects and the local EF Core tool:

   ```bash
   dotnet restore
   dotnet tool restore
   ```

2. Store your local SQL Server connection string outside source control:

   ```bash
   dotnet user-secrets set \
     --project MovieShop.Api/MovieShop.Api.csproj \
     "ConnectionStrings:MovieShop" \
     "Server=127.0.0.1,1433;Database=MovieShop;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True"
   ```

3. Create or update the database from the included migrations:

   ```bash
   dotnet ef database update --project MovieShop.Api/MovieShop.Api.csproj
   ```

4. Start the API in one terminal:

   ```bash
   dotnet run --project MovieShop.Api/MovieShop.Api.csproj --urls http://127.0.0.1:5080
   ```

5. Start the MVC website in another terminal:

   ```bash
   dotnet run --project MovieShop.Web/MovieShopMVC.csproj --urls http://127.0.0.1:5081
   ```

Open `http://127.0.0.1:5081` in a browser. The SQL seed scripts and database
credentials are intentionally not stored in this repository.

