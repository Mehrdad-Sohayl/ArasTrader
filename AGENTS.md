# ArasTrader — Agent Guide

## Project overview

ASP.NET Core 10.0 (preview) trading system following Clean Architecture.
Two runtimes share the same codebase: an HTTP API and a Hangfire background worker.
PostgreSQL is the sole database; Docker Compose is the standard deployment method.

## Build & run

All commands run from `src/` (where `ArasTrader.slnx` lives).

```bash
dotnet build ArasTrader.slnx
dotnet run --project ArasTrader.Api        # API on http://localhost:5126 (dev)
dotnet run --project ArasTrader.Worker     # Hangfire worker
```

Docker Compose (from repo root):

```bash
docker compose up --build    # requires .env with ARAS_API_USERNAME / ARAS_API_PASSWORD
docker compose down -v       # -v drops the postgres volume
```

## Solution structure

```
src/
├── ArasTrader.Domain/         # Entities (Customer, Order, Wallet), domain exceptions, Result<T>
├── ArasTrader.Application/    # Service interfaces, DTOs, application services, DI registration
├── ArasTrader.Infrastructure/ # EF Core, Refit clients, Hangfire, repositories, migrations
├── ArasTrader.Api/            # Controllers, middleware, Swagger, Hangfire Dashboard
├── ArasTrader.Worker/         # Hosts Hangfire Server, registers recurring OrderProcessingJob
└── ArasTrader.slnx            # Solution file (XML-based .slnx format, not classic .sln)
```

Dependency direction: `Api/Worker → Infrastructure → Application → Domain` (Domain has no project references).

## Key architectural facts

- **DI entry points**: `AddApplication()` and `AddInfrastructure(IConfiguration)` — both are extension methods in their respective `DependencyInjection.cs` files.
- **Migrations auto-apply**: The API runs `dbContext.Database.Migrate()` at startup (`Program.cs`). No manual migration step needed.
- **DbContext**: Single context `ArasTraderDbContext` in `Infrastructure/Persistence/Contexts/`.
- **Migrations location**: `Infrastructure/Migrations/` — currently one snapshot + one migration (`InitialCreate`).
- **EF entity configurations**: `Infrastructure/Persistence/Configurations/` — uses `ApplyConfigurationsFromAssembly`.
- **UnitOfWork pattern**: `IUnitOfWork` wraps `SaveChangesAsync` and translates `DbUpdateConcurrencyException` into a domain `ConcurrencyException`.
- **PostgreSQL-specific**: `xmin` column mapped as optimistic concurrency token on Order and Wallet. Raw SQL with `FOR UPDATE SKIP LOCKED` in `PostgresOrderClaimService`.
- **DateTime handling**: All `DateTime` properties are converted to UTC via a global value converter in `OnModelCreating`.
- **Order Gateway**: `IOrderGateway` sits between controllers and `IOrderService` — designed as an extensibility point for future channels (FIX, gRPC, etc.).
- **Token management**: `TokenManager` with three-tier fallback: memory cache → database → refresh → full re-auth. Protected by `SemaphoreSlim`.
- **External API**: Refit client `IArasApiClient` calls `https://interview.arasetrader.ir` for auth and customer data.
- **Result pattern**: Domain uses `Result<T>` with `DomainError` records. Application exceptions map to HTTP 400, domain exceptions to 422, infrastructure exceptions to 500.

## Configuration

- **Connection string**: `ConnectionStrings:DefaultConnection` (PostgreSQL).
- **Aras API creds**: `ArasApi:Username` / `ArasApi:Password` — in Docker these come from `ARAS_API_USERNAME` / `ARAS_API_PASSWORD` env vars via the `.env` file.
- **Order processing**: `OrderProcessing` section — `BatchSize`, `ProcessingTimeout` (TimeSpan), `CronExpression`.
- **Hangfire**: `Hangfire:SchemaName` — defaults to `hangfire` schema.
- **Launch settings**: API dev server on `http://localhost:5126` (not 8080; 8080 is the Docker port).

## Important notes

- **No test projects** exist in the solution. Verification is manual via Swagger (`/swagger`) or the Hangfire Dashboard (`/hangfire`).
- **No `global.json`**, no `Directory.Build.props`, no `.editorconfig`, no `nuget.config` — defaults apply.
- **No CI/CD workflows** — `.github/workflows/` is empty.
- **`.env` at repo root** contains real credentials — do not commit new secrets.
- **`mimo-project/`** directory at repo root is empty and unrelated to the .NET solution.
- **`OrderProcessingOptions.SectionName`** is `"OrderProcessor"` but the Worker's `appsettings.json` uses `"OrderProcessing"` — verify the binding resolves correctly when changing these settings.
- **Worker `Worker.cs`** is a stub `BackgroundService` that just logs — the real background work is done by Hangfire's `OrderProcessingJob`, not this class.

## Adding new features

- New domain entities go in `Domain/Entities/` with their own configuration in `Infrastructure/Persistence/Configurations/`.
- New application services need an interface in `Application/Interfaces/` and registration in `Application/DependencyInjection.cs`.
- New infrastructure implementations register in `Infrastructure/DependencyInjection.cs`.
- New API endpoints go in `Api/Controllers/` with request/response contracts in `Api/Contracts/`.
- After changing entity models, add a migration: `dotnet ef migrations add <Name> --project ArasTrader.Infrastructure --startup-project ArasTrader.Api` (run from `src/`).
