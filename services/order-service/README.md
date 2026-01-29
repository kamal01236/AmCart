# Order Service

Handles order placement and retrieval for customers. Aligned with ADRs:
- [ADR-001: Microservices](../../docs/decisions/adr-001-microservices.md)
- [ADR-003: Azure AD Identity Provider](../../docs/decisions/adr-003-azure-ad.md)
- [ADR-004: Event Streaming Platform (Kafka)](../../docs/decisions/adr-004-event-streaming.md)
- [ADR-004: Azure Kubernetes Service](../../docs/decisions/adr-004-aks.md)
- [ADR-007: Relational Database Platform (PostgreSQL)](../../docs/decisions/adr-007-postgresql.md)

## Responsibilities
- Accept orders from authenticated callers (Azure AD tokens) and persist to PostgreSQL (service-owned schema).
- Emit `OrderPlaced` domain events to Kafka for downstream services (inventory, payments, notifications).
- Expose order retrieval by order number and by customer.

## Tech Stack
- ASP.NET Core 8 Web API
- Microsoft.Identity.Web for Azure AD JWT validation
- PostgreSQL via EF Core (no shared schemas with other services)
- Kafka producer (Confluent.Kafka) for events
- Kubernetes/AKS deployment via Helm; secrets resolved through Key Vault / managed identity

## Configuration
- `ConnectionStrings__Orders`: PostgreSQL connection string (from Key Vault/secret store)
- `AzureAd`: Azure AD authority, client ID, tenant, audience
- `Messaging__Kafka__BootstrapServers`: Kafka bootstrap servers (private endpoints)
- `Messaging__Kafka__OrderTopic`: Topic for `OrderPlaced` events

## Endpoints
- `GET /api/orders/{orderNumber}` – fetch order by number
- `GET /api/orders/customer/{customerId}` – list orders for a customer
- `POST /api/orders` – place an order (publishes Kafka event)

## Events
- Topic: `Messaging__Kafka__OrderTopic`
- Payload: `OrderPlacedEvent` (order id, number, customer id, status, total, createdAtUtc, items)

## Testing
```
dotnet test OrderService.sln
```