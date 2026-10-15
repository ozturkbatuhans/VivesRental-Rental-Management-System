<div align="center">

# 🎬 VivesRental

### Rental Management System — ASP.NET Core Web API + Blazor WebAssembly

A full-stack application for managing a rental shop: products, physical items, customers, rental orders, returns and reservations, all behind a JWT-secured REST API.

![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-13-239120?logo=csharp&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-512BD4?logo=blazor&logoColor=white)
![EF Core](https://img.shields.io/badge/EF%20Core-9-6DB33F)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver&logoColor=white)
![JWT](https://img.shields.io/badge/Auth-JWT-000000?logo=jsonwebtokens&logoColor=white)
![Tests](https://img.shields.io/badge/tests-MSTest%20%7C%20xUnit-blue)

</div>

---

## 📖 Overview

**VivesRental** is a rental management application built as an individual project. It covers the complete rental flow of a shop that lends out items (books, games, equipment, …):

1. A **product** describes a type of item (e.g. *"Board game X"*).
2. Each product has one or more **articles** — the physical copies that can actually be rented.
3. A **customer** can **reserve** an article for a period, or come to the desk and **rent** it.
4. Renting creates an **order** with **order lines**; each line has a due date based on the product's rental period.
5. When items come back, they are **returned** per line or for the whole order at once.

The solution follows a layered architecture with a clear separation between the domain model, data access, business logic, the HTTP API and the UI.

## ✨ Features

- **Dashboard** — start a new rental, open an order directly by its ID to process a return, and quick links to every module.
- **Products** — full CRUD, rental period per product (`RentalExpiresAfterDays`) and **bulk generation of articles** for a product.
- **Articles** — manage physical items and their status (`Normal`, `Broken`, `InRepair`, `Lost`, `Destroyed`); only *Normal* items can be rented or reserved.
- **Customers** — full CRUD with detail pages showing their orders and reservations.
- **Rentals** — create an order for a customer and rent articles one by one, in bulk, or "any available item of this product".
- **Returns** — return a single order line or every open line of an order.
- **Reservations** — reserve an article for a time window with **overlap detection**; open-ended reservations are supported.
- **Availability logic** — an article is available only if it is in *Normal* status, not currently rented and not reserved for that moment (implemented as composable LINQ expression trees, so it is translated to SQL).
- **Order snapshots** — orders and order lines store a copy of the customer and product data, so history stays intact even when a customer is deleted (GDPR-friendly).
- **Authentication** — JWT bearer tokens; destructive operations require the `Admin` role.
- **Swagger UI** with bearer-token support for exploring and testing the API.
- **Resilient UI** — loading/error states and a global error boundary with a retry option when the API is unreachable.

## 🏗️ Architecture

```
┌──────────────────────────┐        HTTPS + JWT         ┌──────────────────────────┐
│   VivesRental.Blazor     │  ───────────────────────▶  │     VivesRental.Api      │
│  Blazor WebAssembly SPA  │       JSON REST calls       │  ASP.NET Core Web API    │
└──────────────────────────┘                            └────────────┬─────────────┘
                                                                     │
                                         ┌───────────────────────────▼───────────────────────────┐
                                         │ VivesRental.Services  (business logic)                │
                                         │   ├─ Services.Abstractions  (interfaces)              │
                                         │   └─ Services.Model         (requests/results/filters)│
                                         └───────────────────────────┬───────────────────────────┘
                                                                     │
                                         ┌───────────────────────────▼───────────────────────────┐
                                         │ VivesRental.Repository  (EF Core DbContext+migrations)│
                                         └───────────────────────────┬───────────────────────────┘
                                                                     │
                                                              SQL Server
```

| Project | Responsibility |
|---|---|
| `VivesRental.Api` | REST controllers, JWT authentication, CORS, Swagger, dependency injection |
| `VivesRental.Blazor` | Blazor WebAssembly front-end (pages, typed HTTP services, auth state) |
| `VivesRental.Services` | Business logic: renting, returning, reservations, validation, mapping |
| `VivesRental.Services.Abstractions` | Service interfaces consumed by the API |
| `VivesRental.Services.Model` | DTOs: request, result and filter objects |
| `VivesRental.Repository` | `DbContext`, model configuration and EF Core migrations |
| `VivesRental.Model` | Domain entities (`Product`, `Article`, `Customer`, `Order`, `OrderLine`, `ArticleReservation`) |
| `VivesRental.Model.Extensions` | Reusable LINQ expressions (`IsAvailable`, `IsRented`, `IsReserved`) and a `PredicateBuilder` |
| `VivesRental.Enums` | Shared enums (`ArticleStatus`) |
| `VivesRental.Model.Tests` / `VivesRental.Services.Tests` | Unit tests with MSTest / xUnit (EF Core InMemory) |
| `VivesRental.Tests.Data` | Test data factories shared by the test projects |

### Domain model

```
Product 1 ── * Article 1 ── * OrderLine * ── 1 Order * ── 0..1 Customer
                  │                                              │
                  └──────── * ArticleReservation * ──────────────┘
```

## 🛠️ Tech Stack

- **Back-end:** ASP.NET Core 9 Web API, Entity Framework Core 9, SQL Server
- **Front-end:** Blazor WebAssembly, Bootstrap 5, Blazored.LocalStorage
- **Security:** JWT bearer authentication, role-based authorization
- **API docs:** Swashbuckle / Swagger UI
- **Testing:** MSTest, xUnit, Moq, EF Core InMemory provider
- **CI:** GitHub Actions (build + test on every push)

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server LocalDB (ships with Visual Studio) or any SQL Server instance
- Optional: `dotnet-ef` tool — `dotnet tool install --global dotnet-ef`

### 1. Clone

```bash
git clone https://github.com/ozturkbatuhans/VivesRental-Rental-Management-System.git
cd VivesRental-Rental-Management-System
```

### 2. Configure the database

The default connection string in `VivesRental.Api/appsettings.json` targets LocalDB:

```json
"ConnectionStrings": {
  "VivesRentalDb": "Server=(localdb)\\MSSQLLocalDB;Database=VivesRental;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
}
```

Create the database with the included EF Core migrations:

```bash
dotnet ef database update --project VivesRental.Repository --startup-project VivesRental.Api
```

> Alternatively, run `database/schema.sql` on an empty SQL Server database (adjust the `USE` statement to your database name).

### 3. Run the API and the front-end

```bash
# terminal 1 – API (https://localhost:7073, Swagger at /swagger)
dotnet run --project VivesRental.Api --launch-profile https

# terminal 2 – Blazor UI (https://localhost:7004)
dotnet run --project VivesRental.Blazor --launch-profile https
```

In Visual Studio you can also set **multiple startup projects** (`VivesRental.Api` + `VivesRental.Blazor`).

### 4. Log in

| Username | Password |
|---|---|
| `admin` | `admin` |

> ⚠️ The demo credentials and the JWT signing key in `appsettings.json` are for **local development only**. For any real deployment, move the key to user secrets / environment variables and replace the hard-coded login with a proper user store (e.g. ASP.NET Core Identity).

## 🔌 API Endpoints

All endpoints except `POST /api/auth/login` require a bearer token.

| Resource | Method & Route | Description |
|---|---|---|
| Auth | `POST /api/auth/login` | Get a JWT |
| Products | `GET /api/products` · `GET /api/products/{id}` | List (with filters) / details |
| | `POST /api/products` · `PUT /api/products/{id}` · `DELETE /api/products/{id}` | Create / update / delete (*Admin*) |
| | `POST /api/products/{id}/articles/generate` | Generate N articles for a product |
| Articles | `GET /api/articles` · `GET /api/articles/{id}` | List (with availability filters) / details |
| | `POST /api/articles` · `PUT /api/articles/{id}/status` · `DELETE /api/articles/{id}` | Create / change status / delete |
| Customers | `GET` · `POST /api/customers`, `GET` · `PUT` · `DELETE /api/customers/{id}` | Customer CRUD (delete: *Admin*) |
| Orders | `GET /api/orders` · `GET /api/orders/{id}` · `POST /api/orders` | List / details / create for a customer |
| | `POST /api/orders/{id}/return` | Return all open lines of an order |
| Order lines | `GET /api/orderlines` · `GET /api/orderlines/{id}` | List / details |
| | `POST /api/orderlines/rent` · `/rent/bulk` · `/rent/by-product` | Rent one article, several articles, or any available article of a product |
| | `POST /api/orderlines/{id}/return` | Return a single line |
| Reservations | `GET` · `POST /api/articlereservations`, `GET` · `DELETE /api/articlereservations/{id}` | Reservations with overlap check |
| Time | `GET /api/time/utcnow` | Server time (used by the UI) |

Full request/response schemas are available in Swagger UI when the API runs in the *Development* environment.

## 🧪 Tests

```bash
dotnet test
```

The test suites cover the availability/rental/reservation expressions, service behaviour (products, articles, orders, order lines), validation rules and the LINQ projection mappings, using the EF Core InMemory provider.

## 📁 Repository Structure

```
├── .github/workflows/          # CI pipeline (build + test)
├── database/schema.sql         # SQL Server schema script
├── VivesRental.Api/            # Web API
├── VivesRental.Blazor/         # Blazor WebAssembly client
├── VivesRental.Services*/      # Business logic, interfaces, DTOs
├── VivesRental.Repository/     # EF Core DbContext + migrations
├── VivesRental.Model*/         # Domain entities + expression extensions
├── VivesRental.Enums/          # Shared enums
├── VivesRental.*Tests*/        # Unit tests + test data factories
├── global.json
└── VivesRental.sln
```

## 👤 Author

**Ahmet Batuhan Öztürk**
- GitHub: [@ozturkbatuhans](https://github.com/ozturkbatuhans)
- LinkedIn: [ozturkofahmet](https://www.linkedin.com/in/ozturkofahmet)
