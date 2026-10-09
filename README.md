# KarinderyaOrderApp

A small ASP.NET Core MVC app for a karinderya (small eatery). Staff manage the menu (foods) and take orders against live stock.

## How to run

**You need:** Visual Studio 2026, the .NET 10 SDK (installed with Visual Studio), and SQL Server 2022 (Express, Developer, or LocalDB).

**1. Get the code**
In Visual Studio: on the start window click **Clone a repository**, paste this URL, pick a folder, then click **Clone**:

```
https://github.com/kingdavidperalta/KarinderyaOrderApp.git
```

Terminal alternative (needs [Git](https://git-scm.com/downloads)):

```
git clone https://github.com/kingdavidperalta/KarinderyaOrderApp.git
cd KarinderyaOrderApp
```

Then open `KarinderyaOrderApp.sln` in Visual Studio (File > Open > Project/Solution).

**2. Set the connection string**
The connection string is left blank in `appsettings.json` on purpose, so no credentials are committed:

```json
"ConnectionStrings": {
  "KARINDERYA_CONNECTION_STRING": ""
}
```

Fill in the value (keep the key name exactly as it is). For SQL Server on your own PC, this works for most setups:

```
Server=localhost;Database=KarinderyaOrderDb;Trusted_Connection=True;TrustServerCertificate=True;
```
**3. Create the database**
In Visual Studio: **Tools > NuGet Package Manager > Package Manager Console**, make sure the "Default project" is `KarinderyaOrderApp`, then copy and paste below:

```
Update-Database
```

This creates the database and tables from the migrations.

Prefer the terminal? From the project folder run `dotnet ef database update` (install the tool once with `dotnet tool install --global dotnet-ef`).

**4. Run it**
Press **F5** (or the green Start button). The browser opens automatically.
Terminal alternative: `dotnet run`, then open the URL printed in the console.

**5. Try it out**
Go to **Foods > Add** and create a couple of foods with stock, then open **Orders**.

**Troubleshooting**

| Problem | Fix |
|---|---|
| `A network-related or instance-specific error` | SQL Server isn't running or the `Server=` value is wrong. Check SQL Server (SQLEXPRESS) is started in Services. |
| `The certificate chain was issued by an authority that is not trusted` | Add `TrustServerCertificate=True;` to the connection string. |
| `Update-Database` says no migrations / unknown command | Confirm the Default project is `KarinderyaOrderApp`, and that the `Migrations` folder exists. |
| `Cannot open database` / login failed | Your Windows or SQL user needs permission to create databases on that server. |

## How to run tests or checks

There is no automated test project. Checks done:

- `dotnet build` (no errors)
- Manual walkthrough: create/edit/archive/restore a food, place an order, confirm stock drops and the order shows under Orders.

## Assumptions

- Single-site, single-currency (₱) use by trusted staff, so there is no login.
- Food names are unique, compared case-insensitively.
- An order is a set of foods with quantities. There is no customer name, table or payment info.
- A food with stock 0 can't be ordered and hidden from order menu.

## Important design decisions

- **Layers:** controllers use `AppDbContext` directly. Each screen has its own view model. Entity mapping lives in `IEntityTypeConfiguration` classes.
- **No overselling:** order creation runs in a transaction. Stock is checked and reduced in one conditional `ExecuteUpdateAsync`, so two simultaneous orders can't both take the last item.
- **Price snapshot:** `OrderItem.UnitPrice` copies the price at order time, so later price edits don't change past orders.
- **Archive instead of delete:** `OrderItem → Food` is `Restrict`, so foods with order history can't be hard-deleted. Foods are archived and can be restored. Archived foods are hidden from new orders.
- **Duplicate lines:** if the same food appears twice in a submitted order, quantities are summed.
- **Search and paging:** server-side, shared `PagedViewModel` (4 per page). Foods search name/description; orders search order #, status or food name.
- **Safety basics:** server-side validation, anti-forgery tokens on every POST, `AsNoTracking` on read queries.

## Known limitations

- No authentication or authorization.
- Orders are always `Pending`. There is no way to complete or cancel one, and nothing restores stock. (The orders list already has badge styling for `Completed` and `Cancelled`.)
- Editing a food overwrites `QuantityInStock`, which could clobber a stock change from an order placed at the same moment.
- Name uniqueness is checked in code. Add a unique index on `Food.Name` to make it fully safe under concurrent requests.
- `ToLocalTime()` runs on the server, so users see server time, not their own.
- The New Order page's cart script (`@section Scripts` in `Views/Order/Create.cshtml`) is empty, so the cart and Place Order button do not work yet. `[REMOVE THIS LINE once the script is added]`
- No automated tests.

## Features I chose not to build

- **Docker / Dockerfile:** I skipped containerizing the app because my development machine has very little free storage, and Docker images plus SQL Server containers take several GB. The app runs directly through Visual Studio or `dotnet run` against a local SQL Server instead (see "How to run"). A Dockerfile and `docker-compose.yml` with a SQL Server container would be the next step for easier setup.
- Customer details, payments, order editing/cancelling, user accounts and roles, reports, and an API. `[ADJUST to match your time/scope limit]`

## How I used AI

- **Razor views:** I used AI to help create the views (the pages for foods and orders, and the layout). I reviewed them and connected them to my own controllers and view models.
- **README:** AI helped draft this README from my code. I checked it against the project and edited it.
