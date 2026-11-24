# Guitar Shop

An ASP.NET Core MVC web application for a musical instrument shop. Customers can browse guitars, basses and drums, view product details with discounted prices, and add items to a shopping cart. An admin area lets the shop owner manage products and categories.

Built by Solomon Rantlhago as part of the Internet Programming course.

## Features

**Customer side**
- Browse all products or filter by category (Guitars, Basses, Drums, or "Strings" for guitars and basses together)
- Product details page with list price, 20% discount and the customer's savings
- Shopping cart stored in session: add items, see quantities, subtotals and a total, remove items or clear the cart
- Prices displayed in South African rands (R)

**Admin side** (`/AdminHome`)
- Add, update and delete products
- Upload a product picture when adding or updating a product
- Add, update and delete categories

## Technologies

- ASP.NET Core MVC (.NET 10) and C#
- Entity Framework Core with SQL Server (LocalDB)
- Razor views, HTML, CSS and Bootstrap
- Repository pattern with a repository wrapper for data access
- Session state for the shopping cart

## Project structure

```
GuitarShop/
├── Controllers/     Home, Product, Cart, AdminHome, AdminProduct, AdminCategory
├── Data/            DbContext, repositories and seed data
├── Models/          Product, Category and view models
├── Migrations/      EF Core migrations
├── Views/           Razor views (Home, Product, Cart, Admin*, Shared)
└── wwwroot/         CSS, libraries and product images
```

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server Express LocalDB (included with the Visual Studio "ASP.NET and web development" workload)
- The EF Core command-line tool:
  ```
  dotnet tool install --global dotnet-ef
  ```

### Run the application

1. Open a terminal in the project folder (the folder that contains `GuitarShop.csproj`).
2. Create the database:
   ```
   dotnet ef database update
   ```
3. Start the app:
   ```
   dotnet run
   ```
4. Open http://localhost:5000 in your browser.

The database is seeded with sample categories and products the first time the app starts.

The connection string is in `appsettings.json` and uses `(localdb)\mssqllocaldb` by default.

## Using the app

| Page | URL |
|---|---|
| Home | `/` |
| All products | `/Product/List` |
| Products in a category | `/Product/List/Guitars` |
| Shopping cart | `/Cart` |
| Admin home | `/AdminHome` |

Product pictures are stored in `wwwroot/images` and matched to a product by its **Code**, using the names `{code}_s.png`, `{code}_m.png` and `{code}_l.png`. Uploading a picture in the admin form saves it under the product's code automatically.

## Notes

- The cart is kept in the browser session, so it is cleared when the session ends or the app restarts. Orders are not saved to the database and there is no checkout.
- The admin area has no login. It is intended for local development and course work only.
