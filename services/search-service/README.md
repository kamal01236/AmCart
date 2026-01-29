# Search Service

Provides a thin API facade over the product search index (Elasticsearch/Azure Cognitive Search), enforcing authorization and returning normalized search results for the web and admin portals. The service fans out read traffic from the frontend so the search cluster stays isolated from public access.

## API Surface
- `GET /api/search/products` – executes keyword/filtered searches with pagination, sorting, and tag/category filters. Query parameters mirror `ProductSearchRequest`.

All endpoints require Azure AD-issued bearer tokens. Admin-specific routes can be layered later via scopes/roles.

## Configuration
| Setting | Description |
| --- | --- |
| `AzureAd` | Standard Microsoft.Identity.Web settings (Instance, Domain, TenantId, ClientId, Audience). |
| `SearchIndex:Endpoint` | Base URL of the search cluster (e.g., `https://my-es.eastus.azure.elastic-cloud.com/`). |
| `SearchIndex:IndexName` | Index or collection that stores product documents. |
| `SearchIndex:ApiKey` | API key header (`api-key`) for Azure Cognitive Search style authentication. |
| `SearchIndex:Username`/`Password` | Optional Basic Auth pair for managed Elasticsearch clusters. |

Provide these values via `appsettings.*`, environment variables (double underscore notation), or Kubernetes secrets as wired in the Helm chart.

## Local Development
1. Ensure an accessible search backend (Elastic or Azure Cognitive Search) with test data.
2. Update `SearchIndex` settings in `src/appsettings.json` or user secrets.
3. Configure Azure AD values or disable auth temporarily for local testing.
4. Run the API: `dotnet run --project src/SearchService.Api.csproj` and call `https://localhost:5001/api/search/products` with a bearer token.

## Observability & Resilience
- HTTP client uses Polly-based retries + circuit breaker to guard against transient failures.
- `/healthz` endpoint surfaces readiness/liveness probes.
- Structured logs emit query metadata (page/size) without recording raw user queries beyond trace logs.

## Tests
```
dotnet test SearchService.sln
```
