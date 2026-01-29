# Application Architecture (SPA + Microservices)

## Frontend (Angular SPA)
- MSAL (Authorization Code + PKCE) against Azure AD B2C; scopes from environment tfvars.
- State: NgRx; API clients generated from service OpenAPI specs.
- Build/test: lint + unit (Jest), e2e (Cypress); artifacts packaged as nginx image for AKS ingress.
- Config: environment files sourced from Key Vault secrets (`aad-spa-client-id-*`, `aad-scope-*`, API base URLs per region).

## Backend (.NET 8 Microservices)
- Services: Auth, Catalog, Order, Payment, Inventory, Search, Notifications.
- Cross-cutting libs: libraries/common-auth, libraries/common-config, libraries/common-events, libraries/common-logging.
- Data ownership: each service owns its schema; EF Core migrations per bounded context; no shared DBs (see ADR-001, ADR-007, ADR-008).
- Async: Kafka/Event Hubs for domain events; producers/consumers follow shared conventions.

## Security
- Gateway enforces JWT validation (Azure AD/AAD B2C) and scopes/roles (ADR-003).
- Secrets via Key Vault; pod-managed identities; TLS everywhere; WAF in prevention mode.

## Observability
- Structured logging to App Insights/Log Analytics; metrics + traces with OpenTelemetry exporters.
- SLOs and alerts defined in observability/.

## Resilience
- HPAs on CPU/memory/RPS/Kafka lag; Polly on outbound calls; circuit breakers on external deps; readiness/liveness endpoints exposed.

## Logical Diagram
```mermaid
flowchart LR
  User --> FrontDoor[Front Door / Traffic Manager]
  FrontDoor --> AppGw[App Gateway WAF v2 (per region)]
  AppGw --> Ingress[AKS Ingress Controller]
  Ingress --> APIGW[API Gateway (in-cluster)]
  APIGW --> Services[.NET 8 Services]
  Services -->|Sync| Postgres[(PostgreSQL)]
  Services -->|Async| Kafka[(Kafka/Event Hubs)]
  Services --> Redis[(Redis Cache)]
  Services --> Search[(Elasticsearch)]
  Services --> Cassandra[(Cassandra)]
  Services --> KeyVault[(Key Vault)]
  Services --> AppInsights[(App Insights/Log Analytics)]
```
