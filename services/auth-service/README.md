# Auth Service

Provides Azure AD-backed session bootstrap and profile management for AmCart clients. The service validates incoming Microsoft Entra ID (Azure AD / B2C) tokens, persists user profile metadata, and exposes APIs for the SPA to exchange a freshly issued ID token for an AmCart session context.

## Endpoints
-
- `POST /api/auth/session` (requires `Authorization: Bearer <AAD token>`): validates the token, upserts the user profile, and returns session metadata (display name, email, roles, last login timestamp).
- `GET /api/auth/profile` (requires auth): fetches the persisted profile for the current principal.

## Technology
- ASP.NET Core 8 Web API
- Microsoft.Identity.Web for Azure AD token validation
- PostgreSQL (via Entity Framework Core + Npgsql) for profile persistence
- Containerized via provided Dockerfile; Helm manifests live under `helm/` (to be wired into the platform charts).

## Local Development
1. Ensure PostgreSQL is available and update `ConnectionStrings:Auth` in `src/appsettings.Development.json` if needed.
2. Set Azure AD env vars: `AzureAd__TenantId`, `AzureAd__ClientId`, `AzureAd__Audience`.
3. Run database migrations (pending) or `dotnet ef database update` once migrations exist.
4. Launch via `dotnet run --project src/AuthService.Api.csproj` and browse `https://localhost:7186/swagger`.

## Testing
```
dotnet test AuthService.sln
```
