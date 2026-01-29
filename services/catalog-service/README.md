# Catalog Service

Manages product and category data, enforces CRUD flows for merchandising teams, and emits product change events for downstream search indexing. All APIs require Azure AD-issued bearer tokens, with administrative actions restricted to principals that hold `Catalog.Admin`.

## API Surface
- `GET /api/catalog/products?query=*&categoryId=` – server-side filtering/search with pagination.
- `GET /api/catalog/products/{id}` – fetch a single product record.
- `POST /api/catalog/products` – create a product (admin only).
- `PUT /api/catalog/products/{id}` – update details (admin only).
- `POST /api/catalog/products/{id}/index` – force a reindex event.
- `GET /api/catalog/categories` – list all categories.
- `POST /api/catalog/categories` / `PUT /api/catalog/categories/{id}` – manage categories (admin only).

## Tech Stack
- ASP.NET Core 8 Web API
- Entity Framework Core + PostgreSQL storage
- Azure Event Hubs producer for search-indexer fan-out
- JWT auth via Microsoft.Identity.Web

## Local Development
1. Provision PostgreSQL (or run via Docker) and update `ConnectionStrings:Catalog`.
2. Export Azure AD settings via appsettings or environment variables.
3. Apply EF migrations (pending) using `dotnet ef database update`.
4. Run `dotnet run --project src/CatalogService.Api.csproj` and exercise endpoints via Swagger.

## Tests
```
dotnet test CatalogService.sln
```
