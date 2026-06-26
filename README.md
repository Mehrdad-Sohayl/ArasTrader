# ArasTrader Technical Assignment

## Overview

This project was developed as a technical assignment to implement a small trading workflow using a provided external API.

The system synchronizes customers from the external service, manages customer wallets, accepts buy and sell orders, and processes those orders asynchronously through a scheduled background job.

The implementation focuses on:

- External API integration
- Customer synchronization
- Wallet management
- Order lifecycle management
- Background order processing
- Concurrency handling
- PostgreSQL persistence
- Docker-based deployment

The solution is implemented using ASP.NET Core, PostgreSQL, EF Core, Hangfire, and Refit, following a Clean Architecture structure.

### Implemented Scope

- Authentication against the external Aras API
- Customer synchronization and persistence
- Automatic wallet creation for new customers
- Order creation and modification
- Wallet balance reservation and settlement
- Scheduled order processing using Hangfire
- Optimistic concurrency control
- Atomic order claiming using PostgreSQL `FOR UPDATE SKIP LOCKED`
- Recovery of timed-out in-progress orders

### Not Implemented

The following capabilities are not implemented in the current solution:

- Idempotency
- Refresh token flow
- API versioning
- Distributed messaging
- CQRS
- Event sourcing
- Distributed caching

## Architecture

The solution is organized into five projects following a layered architecture.

### Solution Structure

```text
ArasTrader.Api
 ├─ ArasTrader.Application
 └─ ArasTrader.Infrastructure

ArasTrader.Worker
 ├─ ArasTrader.Application
 └─ ArasTrader.Infrastructure

ArasTrader.Infrastructure
 ├─ ArasTrader.Application
 └─ ArasTrader.Domain

ArasTrader.Application
 └─ ArasTrader.Domain

ArasTrader.Domain
```

### Project Responsibilities

| Project                   | Responsibility                                                                                                                                               |
| ------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| ArasTrader.Api            | Hosts HTTP endpoints, middleware pipeline, Swagger UI, and Hangfire Dashboard                                                                                |
| ArasTrader.Application    | Contains application services, workflow orchestration, and service abstractions                                                                              |
| ArasTrader.Domain         | Contains domain entities, aggregates, domain validation, and business invariants                                                                             |
| ArasTrader.Infrastructure | Provides persistence, repository implementations, external API integration, token management, Hangfire configuration, and PostgreSQL-specific infrastructure |
| ArasTrader.Worker         | Hosts the Hangfire Server and executes scheduled background jobs                                                                                             |

### Dependency Direction

The Domain project contains the core business model and does not depend on other projects within the solution.

The Application layer depends on Domain and defines the workflows used by the system through application services and abstractions.

The Infrastructure layer depends on both Application and Domain, providing implementations for repositories, external integrations, token management, background processing infrastructure, and database access.

The API and Worker projects act as composition roots and register dependencies through dependency injection.

---

### Dependency Injection

Application services are registered through `AddApplication()` and include:

* Customer synchronization services
* Order management services
* Wallet management services
* Order processing services

Infrastructure components are registered through `AddInfrastructure()` and include:

* EF Core DbContext
* Repository implementations
* Unit of Work
* PostgreSQL order claiming service
* Token management services
* Refit-based API client
* Hangfire configuration

### Runtime Components

The system consists of two executable applications:

#### API

Responsibilities:

* Expose HTTP endpoints
* Host Swagger UI
* Host Hangfire Dashboard
* Apply database migrations during startup

#### Worker

Responsibilities:

* Host Hangfire Server
* Execute recurring background jobs
* Process pending orders asynchronously

Both applications use the same PostgreSQL database and share the same Application and Infrastructure registrations.

```text
                        +----------------------+
                        |   External Aras API |
                        +----------+-----------+
                                   ^
                                   |
                              Refit Client
                                   |
+-------------------+              |
|   ArasTrader.Api  |--------------+
+---------+---------+
          |
          | AddApplication()
          | AddInfrastructure()
          v
+-------------------+
|    Application    |
+---------+---------+
          |
          v
+-------------------+
|      Domain       |
+-------------------+
          ^
          |
+---------+---------+
|  Infrastructure   |
+---------+---------+
          |
          |
          v
+-------------------+
|    PostgreSQL     |
+-------------------+
          ^
          |
          |
+---------+---------+
| ArasTrader.Worker |
+---------+---------+
          |
          |
          v
+-------------------+
|     Hangfire      |
+-------------------+
```


---

## Domain Model

The core domain consists of three primary entities:

* Customer
* Wallet
* Order

### Customer

Customers are synchronized from the external Aras API and persisted locally.

A wallet is automatically created for each new customer during the synchronization process.

### Wallet

The Wallet entity represents the customer's available and reserved funds.

Financial state is maintained through two balances:

* `AvailableBalance`
* `ReservedBalance`

All balance modifications are performed through domain methods:

* `ReserveFunds()`
* `ReleaseFunds()`
* `FinalizeReservation()`
* `Credit()`

The entity validates its own invariants and prevents invalid balance transitions by throwing domain exceptions when business rules are violated.

Implemented validations include:

* Positive amount validation
* Sufficient balance checks
* Customer existence validation during creation

Optimistic concurrency is supported through a version field mapped to PostgreSQL's `xmin` column.

### Order

Orders represent buy and sell requests submitted by customers.

Implemented order states:

```text
Pending
→ InProgress
→ Completed

Pending
→ InProgress
→ Rejected
```

State transitions are enforced through domain methods:

* `MarkInProgress()`
* `MarkCompleted()`
* `MarkRejected()`

Orders can be modified only while in the `Pending` state.

Implemented validations include:

* Valid customer identifier
* Non-empty symbol
* Positive quantity
* Positive price
* Valid state transitions

Invalid operations result in domain exceptions.

### Domain Validation

Both Wallet and Order encapsulate their own validation rules and state transitions.

Business rules are enforced through domain methods rather than allowing direct modification of internal state.

State-changing operations are validated before modifications are applied.

---

## Database Schema and Migrations

### Database Provider

The application uses PostgreSQL as its primary persistence store.

Entity Framework Core is used for data access and schema management.

### DbContext

The solution uses a single database context:

```text
ArasTraderDbContext
```

The context is registered in the Infrastructure layer using the Npgsql provider.

### Schema Management

Database schema changes are managed through Entity Framework Core migrations.

Migration files are located in:

```text
ArasTrader.Infrastructure/Migrations
```

The repository includes the migration history required to create the database schema from scratch.

### Applying Migrations

Database migrations are applied during application startup.

The API application executes:

```csharp
dbContext.Database.Migrate();
```

before serving requests.

This ensures that pending migrations are applied automatically when the application starts.

### Core Tables

The database schema includes tables for:

| Table           | Purpose                                                      |
| --------------- | ------------------------------------------------------------ |
| Customers       | Persist synchronized customer data                           |
| Wallets         | Store available and reserved balances                        |
| Orders          | Store buy and sell orders and their processing state         |
| TokenStates     | Persist authentication tokens obtained from the external API |
| Hangfire Tables | Store Hangfire jobs, schedules, and processing metadata      |

### Concurrency Support

Optimistic concurrency control is implemented using PostgreSQL's system-managed `xmin` column.

Protected entities include:

* Orders
* Wallets

Entity Framework Core maps `xmin` as a concurrency token to detect conflicting updates.

### Database Constraints

The schema includes database-level protections in addition to domain validation.

Examples include:

* Non-negative wallet balance constraints
* Entity relationships enforced through foreign keys
* Concurrency metadata managed by PostgreSQL

### PostgreSQL-Specific Features

The current implementation relies on PostgreSQL-specific functionality, including:

* `xmin`-based optimistic concurrency control
* `xid` column mapping
* `FOR UPDATE SKIP LOCKED` for atomic order claiming

As a result, the current schema and concurrency implementation are not database-provider agnostic.

### Creating the Database from Scratch

When running the project through Docker Compose:

1. PostgreSQL is started.
2. The API application starts.
3. Pending migrations are applied automatically.
4. The application becomes ready to serve requests.

No manual migration execution is required for the default Docker-based setup.

### Data Seeding

No database seeding is performed during startup.

All business data is created through application workflows, such as customer synchronization and order creation.

---
## Order Gateway

An Order Gateway layer has been implemented in front of order-related application services.

Current flow:

Controller → Order Gateway → Order Service

The gateway acts as a single entry point for order operations and provides an extensibility point for future order entry channels.

Currently supported channel:

* REST API

Future channels can be added with minimal impact on the business layer:

* FIX adapters
* Message queue consumers
* Partner integrations
* gRPC services

Business rules remain inside the application services while channel-specific concerns are isolated within the gateway layer.

---
## Order Processing Flow

Order processing is executed asynchronously using Hangfire.

A recurring job (`OrderProcessingJob`) runs on the Worker application and periodically processes pending orders.

### Processing Lifecycle

```text
Pending
   |
   v
Claim (FOR UPDATE SKIP LOCKED)
   |
   v
InProgress
   |
   +---- Success ----> Completed
   |
   +---- Failure ----> Rejected
   |
   +---- Timeout ----> Reclaim
```

### Claiming Strategy

Orders are claimed through a PostgreSQL-specific implementation (`PostgresOrderClaimService`).

The claim operation uses:

```sql
FOR UPDATE SKIP LOCKED
```

to prevent multiple workers from claiming the same order concurrently.

Eligible orders include:

* Orders in `Pending` state
* Orders in `InProgress` state whose processing timeout has expired

Claimed orders are updated to `InProgress` as part of the claim operation.

Orders are selected in ascending creation order:

```sql
ORDER BY CreatedAt
```

Batch size is configurable through `OrderProcessingOptions.BatchSize`.

### Processing Outcomes

After an order is claimed, processing is delegated to `IOrderProcessingService`.

The current implementation simulates execution outcomes with a randomized success rate.

#### Successful Buy Order

* Reserved funds are finalized
* Order is marked as `Completed`
* Processing timeout is cleared

#### Successful Sell Order

* Wallet balance is credited
* Order is marked as `Completed`
* Processing timeout is cleared

#### Failed Order

* Reserved funds are released
* Order is marked as `Rejected`
* Processing timeout is cleared

### Failure Recovery

Orders that remain in the `InProgress` state beyond the configured processing timeout become eligible for claiming again.

This mechanism allows processing to continue after worker interruptions or unexpected failures.

Recovery eligibility is determined using:

* Order status (`InProgress`)
* Processing start timestamp (`ProcessingStartedAt`)
* Configured timeout (`OrderProcessingOptions.ProcessingTimeout`)

### Scheduling

The recurring processing job is configured through Hangfire.

Scheduling parameters are provided through configuration:

* `CronExpression`
* `BatchSize`
* `ProcessingTimeout`

The Worker application hosts the Hangfire Server responsible for executing the job, while the API application hosts the Hangfire Dashboard.

---

## Concurrency Strategy

The system uses optimistic concurrency control for domain entities that are subject to concurrent modifications.

### Optimistic Concurrency

Optimistic concurrency is configured for:

* Wallet
* Order

Entity versions are mapped to PostgreSQL's system-managed `xmin` column.

EF Core is configured to use `xmin` as a concurrency token:

* `IsConcurrencyToken()`
* `ValueGeneratedOnAddOrUpdate()`

This allows EF Core to detect conflicting updates during persistence and prevent silent overwrites.

### Wallet Concurrency

The Wallet entity maintains financial state through:

* `AvailableBalance`
* `ReservedBalance`

Concurrency protection helps prevent lost updates when multiple operations attempt to modify wallet balances simultaneously.

In addition to application-level concurrency checks, database constraints enforce non-negative balance values.

### Order Concurrency

Order state transitions are protected through domain validation and optimistic concurrency checks.

Concurrent modifications that would result in conflicting updates are detected during persistence.

### Retry Handling

A retry mechanism is implemented for order editing operations.

Characteristics:

* Triggered by `ConcurrencyException`
* Maximum retry count: `20`
* No delay or backoff strategy
* Explicit clearing of tracked entities between retry attempts

The retry behavior is currently applied to the order editing workflow and is not implemented as a generic retry policy across the system.

### Atomic Order Claiming

Background order processing uses a separate concurrency mechanism at the database level.

Order claiming is performed using PostgreSQL:

```sql
FOR UPDATE SKIP LOCKED
```

This prevents multiple workers from claiming the same order concurrently while allowing other workers to continue processing available work.

This mechanism complements optimistic concurrency by protecting the order-claiming phase of background processing.

---

## External API Integration & Authentication

The system integrates with an external Aras API for authentication and customer synchronization.

Communication with the external service is implemented using Refit.

### Authentication Flow

Access tokens are obtained through the external API and managed by `TokenManager`.

Token retrieval follows the sequence below:

```text
Memory Cache
      ↓
Database
      ↓
Refresh Token
      ↓
External Login
```

When an authenticated request requires an access token, the token manager follows these steps:

1. Attempt to retrieve the token from the in-memory cache.
2. If not found, load the persisted token from PostgreSQL.
3. If the token is still valid, cache it and reuse it.
4. If the token is close to expiration, attempt to refresh it using the previously issued token.
5. If the refresh operation fails (for example, because the previous token has expired), authenticate again using the configured credentials.
6. Persist the newly obtained token and update the in-memory cache.

A 5-minute expiration buffer is applied when validating token lifetime to reduce the risk of using near-expired tokens.

---

### Token Storage

Access tokens are stored using a hybrid persistence strategy.

#### Memory Cache

The in-memory cache is used for fast token retrieval during normal application execution.

Current implementation:

* `MemoryTokenStore`
* Cache key: `ARAS_TOKEN`

#### Database Persistence

Tokens are also persisted in PostgreSQL through `AuthTokenRepository`.

Only a single token record is maintained in the database using an upsert operation.

Stored information includes:

* Access token
* Expiration timestamp
* Creation timestamp
* Modification timestamp

Persisting the token enables the application to preserve authentication state across restarts and continue using the refresh-token flow without requiring an immediate login.

---

### Token Lifecycle

The token manager supports both token refresh and full authentication.

When a token is approaching expiration:

1. The previously issued token is sent to the external refresh endpoint.
2. If the refresh operation succeeds, the new token is persisted and cached.
3. If the refresh operation fails, a new authentication request is issued using the configured credentials.
4. The newly obtained token is persisted and cached.

This strategy minimizes unnecessary authentication requests while allowing automatic recovery from expired sessions.

---

### Concurrency Protection

Token acquisition is protected using `SemaphoreSlim` to prevent multiple concurrent authentication requests from attempting to obtain or refresh a token simultaneously.

---

### Error Handling

Transient communication failures are retried automatically.

Retry characteristics:

* Maximum retries: `3`
* Triggered by:

  * `HttpRequestException`
  * `TaskCanceledException`
* Incremental delay between attempts

Non-transient failures are returned to the caller without retry.

---

### Customer Synchronization

Customer data is retrieved from the external API and stored locally.

During synchronization:

* New customers are persisted
* A wallet is created for each newly discovered customer

The synchronization process is exposed through the customer synchronization endpoint and can be executed on demand.

---

## API Endpoints

The API application exposes endpoints for customer synchronization, wallet management, and order management.

Swagger UI is available for interactive exploration and testing of the API.

### Customer Endpoints

| Method | Route                 |
| ------ | --------------------- |
| POST   | `/api/customers/sync` |

Synchronizes customer data from the external Aras API and persists new customers locally.

---

### Wallet Endpoints

| Method | Route         |
| ------ | ------------- |
| PUT    | `/api/wallet` |

Credits funds to a customer's wallet.

---

### Order Endpoints

| Method | Route              |
| ------ | ------------------ |
| POST   | `/api/orders`      |
| PUT    | `/api/orders/{id}` |

Supported operations:

* Create order
* Edit existing order

Orders are initially created in the `Pending` state and processed asynchronously by the background worker.

### Authentication

No API authentication or authorization requirements are currently configured on the exposed endpoints.

### API Versioning

API versioning is not currently implemented.

### API Documentation

Swagger is enabled in the API application.

Available endpoints:

| Resource           | URL |
|--------------------|-----|
| Swagger UI         | `/swagger` |


Swagger can be used to inspect request models, response models, and endpoint contracts.

## Example Requests

### Synchronize Customers

Synchronizes customers from the external Aras API and persists new customers locally.

**Request**

```http
POST /api/customers/sync
```

Request body is not required.

**Response**

The endpoint does not define a dedicated response DTO.

---

### Create Order

Creates a new order in the `Pending` state.

**Request**

```http
POST /api/orders
Content-Type: application/json
```

```json
{
  "CustomerId": 123,
  "Symbol": "AAPL",
  "Quantity": 100,
  "Price": 150.50,
  "Type": 1
}
```

**Response**

```json
{
  "OrderId": 456
}
```

---

### Edit Order

Updates an existing order.

Only orders in the `Pending` state can be modified.

**Request**

```http
PUT /api/orders/{id}
Content-Type: application/json
```

```json
{
  "Quantity": 50,
  "Price": 160.75
}
```

**Response**

The endpoint does not define a dedicated response DTO.

---

### Deposit Wallet Balance

Credits funds to a customer's wallet.

**Request**

```http
PUT /api/wallet
Content-Type: application/json
```

```json
{
  "CustomerId": 123,
  "Amount": 500.00
}
```

**Response**

The endpoint does not define a dedicated response DTO.

### Notes

* Example values are provided for illustration purposes.
* Request and response contracts should be considered authoritative as defined by the Swagger/OpenAPI specification exposed by the application.


---

## Configuration

Application settings are provided through standard .NET configuration sources and environment variables.

### Required Configuration

#### Database

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=...;Database=...;Username=...;Password=..."
  }
}
```

#### Aras API

```json
{
  "ArasApi": {
    "BaseUrl": "...",
    "Username": "...",
    "Password": "..."
  }
}
```

When running through Docker, credentials can be supplied using environment variables:

```text
ARAS_API_USERNAME
ARAS_API_PASSWORD
```

### Order Processing Configuration

The background order processor is configured through `OrderProcessingOptions`.

| Setting           | Description                                            |
| ----------------- | ------------------------------------------------------ |
| BatchSize         | Maximum number of orders claimed in a processing cycle |
| ProcessingTimeout | Timeout used to reclaim stalled in-progress orders     |
| CronExpression    | Hangfire schedule for recurring order processing       |

Example:

```json
{
  "OrderProcessing": {
    "BatchSize": 100,
    "ProcessingTimeout": "00:05:00",
    "CronExpression": "* * * * *"
  }
}
```

### Hangfire Configuration

Hangfire uses PostgreSQL as its storage backend.

Configuration is loaded through the `HangfireOptions` section and registered during application startup.

### Environment Variables

The Docker deployment uses environment variables for sensitive configuration.

Examples:

```text
ConnectionStrings__DefaultConnection

ArasApi__Username
ArasApi__Password

ASPNETCORE_ENVIRONMENT
DOTNET_ENVIRONMENT
```

### Database Migrations

Entity Framework Core migrations are located in the Infrastructure project.

Database schema updates are applied during application startup using:

```csharp
dbContext.Database.Migrate();
```

This migration step is executed by the API application before serving requests.

### Notes

* PostgreSQL is the only supported database provider in the current implementation.
* Migration files contain PostgreSQL-specific constructs such as `xmin` and Npgsql value generation strategies.
* No database seeding is performed during startup.

---

## Running the Project

### Prerequisites

Required tools:

* Docker
* Docker Compose

The application is designed to run using the provided Docker configuration.

### Environment Variables

Before starting the system, provide the required Aras API credentials:

```bash
export ARAS_API_USERNAME=<username>
export ARAS_API_PASSWORD=<password>
```

Or create a `.env` file:

```text
ARAS_API_USERNAME=<username>
ARAS_API_PASSWORD=<password>
```

### Start the System

From the repository root:

```bash
docker compose up --build
```

This command starts:

* PostgreSQL
* API
* Worker

### Service Endpoints

| Service | URL |
|----------|----------|
| API | http://localhost:8080 |
| Swagger UI | http://localhost:8080/swagger |
| Hangfire Dashboard | http://localhost:8080/hangfire |
| PostgreSQL | localhost:5432 |

### Startup Sequence

During startup:

1. PostgreSQL becomes available.
2. The API application starts.
3. Entity Framework Core migrations are applied.
4. The Worker application starts.
5. Hangfire Server begins processing scheduled jobs.

### Verify the Installation

After startup:

1. Open Swagger UI.
2. Execute customer synchronization.
3. Verify customer and wallet creation.
4. Create an order.
5. Monitor background processing through the Hangfire Dashboard.

### Stopping the System

To stop all services:

```bash
docker compose down
```

To stop services and remove persisted PostgreSQL data:

```bash
docker compose down -v
```

### Notes

* API and Worker run as separate containers.
* PostgreSQL data is persisted using a Docker volume.
* The Worker application is responsible for executing recurring order processing jobs.
* The API application hosts the Hangfire Dashboard and Swagger UI.

---

## Design Decisions and Scope

This project was implemented as a technical assignment with a primary focus on correctness, maintainability, and extensibility rather than feature completeness.

The following design decisions were made intentionally.

### Clean Architecture

The solution follows a layered architecture to separate domain logic, application workflows, infrastructure concerns, and hosting applications. Business rules remain isolated from external frameworks and infrastructure dependencies.

### Authentication

Authentication is handled by a dedicated `TokenManager` that supports:

* Access token caching
* PostgreSQL-backed token persistence
* Automatic token refresh
* Automatic re-authentication when refresh is no longer possible

This approach minimizes unnecessary login requests while allowing authentication state to survive application restarts.

### Order Gateway

An Order Gateway layer has been introduced in front of the order application services to provide a single entry point for order operations.

The current implementation supports REST API requests and provides an extension point for future order channels such as FIX, message queues, partner integrations, or gRPC without impacting the business layer.

### Order Processing

Background order processing is implemented using Hangfire with PostgreSQL storage, as required by the assignment.

Atomic order claiming is implemented using PostgreSQL row locking (`FOR UPDATE SKIP LOCKED`) to prevent multiple workers from processing the same order concurrently.

Timed-out `InProgress` orders are automatically reclaimed to support worker recovery scenarios.

### Concurrency

Optimistic concurrency control is used for wallet and order updates to protect financial consistency during concurrent modifications.

Order claiming is handled separately through database row locking to guarantee exclusive ownership during background processing.

### Out of Scope

The following capabilities were intentionally left outside the scope of this assignment:

* Request idempotency
* API versioning
* Distributed messaging platforms (RabbitMQ, Kafka)
* CQRS and Event Sourcing
* Distributed caching (Redis)

These features can be introduced in future iterations without requiring significant architectural changes.

---