# Eshop Modular Monoliths / Microservices

> **Repository:** `EshopModularMonoliths`

A sample/boilerplate e-commerce platform implementing modern architecture patterns and best practices using .NET 8, C# 12, EF Core, MassTransit, Redis, PostgreSQL, RabbitMQ, Keycloak, and Docker. The solution demonstrates a path from a modular monolith (modulith) to microservices.
<img width="787" height="413" alt="Screenshot 2025-09-13 015316" src="https://github.com/user-attachments/assets/23137714-5558-4f2c-a860-2c775f3d1131" />

---

## Key Architecture & Technical Highlights

- **Modular Monolith (Modulith)** architecture with clear module boundaries.
- **Vertical Slice Architecture (VSA)**: feature folders and single-file feature implementations.
- **Domain-Driven Design (DDD)** and **Clean Architecture** principles inside modules.
- **CQRS** (Command Query Responsibility Segregation) with **MediatR**.
- **Validation pipeline behaviors** using **FluentValidation** + MediatR.
- **Outbox Pattern** for reliable message delivery.
- **Asynchronous messaging** via **RabbitMQ** and **MassTransit**.
- **Synchronous module calls** using in-process public APIs.
- **Caching**: Redis as distributed cache with Cache-aside + Proxy & Decorator patterns.
- **Identity**: OAuth2/OpenID Connect via **Keycloak** (configured in docker-compose).
- **Minimal APIs** with ASP.NET Core (C# 12 / .NET 8 features).
- **Carter** used for endpoint definition for Minimal APIs.
- **Entity Framework Core (Code-First)** with migrations for PostgreSQL.
- **Cross-cutting concerns**: Logging, global exception handling, health checks.
- **Migrate to Microservices**: Estranged/Figure pattern to split modules to microservices.

---

## Modules Overview

- **Catalog**
  - Product catalog, pricing, queries/commands implemented with Vertical Slice and MediatR.
  - Uses EF Core (Postgres) and Carter endpoints.
  - Publishes product price update events to RabbitMQ.

- **Basket**
  - Stores shopping basket in PostgreSQL with Redis as distributed cache.
  - Implements Proxy, Decorator, Cache-aside patterns.
  - Publishes `BasketCheckoutEvent` via MassTransit → RabbitMQ.
  - Uses Outbox pattern for reliable messaging.

- **Ordering**
  - DDD + CQRS + Clean Architecture for order aggregates.
  - Uses Outbox pattern for reliable eventing on checkout.

- **Identity**
  - Keycloak configured (OAuth2 + OpenID Connect) as identity provider.
  - JwtBearer configured for OIDC integration.

- **Module Communications**
  - Sync: in-process public API calls between catalog and basket.
  - Async: RabbitMQ & MassTransit for price update events between modules.

---

## Prerequisites

- Visual Studio 2022 (or VS 2022+)
- .NET 8 SDK or later
- Docker Desktop (running)
- (Optional) Postman for API testing

> Ensure Docker Desktop has enough resources: **Memory: 4 GB**, **CPU: 2** (see Installing section below).

---

## Getting Started (Local Development)

1. **Clone the repository**

```bash
git clone https://github.com/saloma03/EshopModularMonoliths.git
cd EshopModularMonoliths
```

2. **Start Docker (Keycloak, PostgreSQL, Redis, RabbitMQ, etc.)**

From the repository root (where `docker-compose.yml` and `docker-compose.override.yml` live):

```bash
# Use Visual Studio: select docker-compose as startup project and run (or use CLI):
docker-compose -f docker-compose.yml -f docker-compose.override.yml up -d
```

> Allow a few moments for services to become healthy — some microservices and Keycloak may take extra time on first boot.

3. **Configure Docker Desktop resources (Windows)**

Open Docker Desktop → Settings → Resources (or Advanced): set **Memory** to at least **4 GB** and **CPU** to at least **2**.

4. **Run EF Core Migrations (if needed)**

If you prefer to run migrations locally (outside containers) or need to re-run migrations for a specific module, open that module project and run:

```bash
# from module project folder
dotnet ef database update --project <ProjectName>.Infrastructure --startup-project <ProjectName>.Api
```

> Replace placeholders with the appropriate project names in the solution.

5. **Open API**

- Shopping Web API: `https://localhost:6060` (default)

You can import the provided Postman collection (if included in repo) to test endpoints. Internal module endpoints assume internal routing used by the modular monolith — see feature folders for endpoints and request examples.

---

## Configuration

Configuration is handled via `appsettings.json` and environment variables. Important configuration keys (example):

- `ConnectionStrings:Postgres` — PostgreSQL connection string.
- `Redis:Configuration` — Redis connection string.
- `MassTransit:Host` — RabbitMQ host.
- `Keycloak:Authority`, `Keycloak:ClientId`, `Keycloak:ClientSecret` — OIDC settings.
- `Outbox:Settings` — outbox configuration (table names, cleanup intervals).

When running via Docker Compose, many of these are supplied by the compose file or `.env` used by compose — check `docker-compose.yml` for specifics.

---

## Development Notes

- **Feature folders / Vertical slices**: Each endpoint/feature is intentionally kept in a single file (or set of small files) to keep the vertical slice pattern clear and maintainable.
- **MediatR + FluentValidation**: Command/Query handlers use MediatR; validation pipeline behaviors validate incoming requests.
- **Outbox Pattern**: Messages that must be delivered to RabbitMQ are stored in an Outbox table under the same DB transaction as domain changes, then delivered asynchronously.
- **MassTransit**: configured consumers/producers for handling events across modules.
- **Redis Cache**: Basket module uses Redis as distributed cache in cache-aside mode for read performance.

---

## Running Tests

If unit/integration tests are included in solution, run them via Visual Studio Test Explorer or CLI:

```bash
dotnet test
```

Consider spinning up dependent services (Postgres, RabbitMQ, Redis) via Docker Compose before running integration tests.

---

## Postman

There is a Postman collection in the repository (if present). Import it into Postman and set environment variables to match the running environment (Keycloak tokens, API base URL, etc.).

---

## Troubleshooting

- **Services not starting:** check `docker-compose logs <service>` for details.
- **Keycloak login issues:** wait until Keycloak finishes initializing; check Keycloak admin console credentials in `docker-compose` env vars.
- **Database connection errors:** verify `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_DB` used by compose and `ConnectionStrings` in the API apps.
- **RabbitMQ connectivity:** ensure the container is healthy and MassTransit is configured with the correct URI/user/pass.

---

## Roadmap / Migration to Microservices

The solution contains guidance and patterns to split the monolith into true microservices using a staged approach:

1. Keep modules as autonomous projects with well-defined contracts.
2. Replace in-process module calls with HTTP/gRPC calls where appropriate.
3. Use MassTransit + RabbitMQ for asynchronous integration events (already present).
4. Containerize each microservice and use per-service databases if required.
5. Use the Stranger Fig pattern to incrementally migrate modules to independent services.

---

## Contributing

Contributions, improvements and bug fixes are welcome. Open an issue and submit a pull request with a clear description of the change.

---

## License

This repository does not include an explicit license file. Add a `LICENSE` file (MIT, Apache-2.0, etc.) if you want to permit reuse.

---

## Contact

If you want help running or extending the project, open an issue in the repository or contact the maintainer.

---

*README generated for `EshopModularMonoliths` — adjust any commands, ports, or configuration values to match your local setup.*

