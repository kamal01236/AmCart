# High-Level Design (HLD)

## Overview
AmCart is a B2C e-commerce platform delivered as an Angular SPA backed by .NET 8 microservices. Identity is issued by Microsoft Entra ID (Azure AD) for admins/ops/CI and Azure AD B2C for customers ([ADR-003](../decisions/adr-003-azure-ad.md), [ADR-006](../decisions/adr-006-frontend-backend-stack.md)).

## Global vs Regional Architecture
- **Global layer (once):** Azure AD/AAD B2C, Azure DNS/Front Door or Traffic Manager, ACR, Terraform state, GitHub Actions OIDC. No customer workloads run here.
- **Regional stacks (per Azure region: eastus, westeurope, ...):** Application Gateway WAF v2, AKS, PostgreSQL (single-writer, geo-read replicas), Cassandra, Elasticsearch, Kafka, Redis, Key Vault, Log Analytics/Application Insights. Remote state and deployments are isolated per region ([docs/design/ci-cd.md](ci-cd.md)).
- **Principles:** No synchronous cross-region calls; data residency respected; failover via global routing; infra code and state isolated per region ([ADR-001](../decisions/adr-001-microservices.md), [ADR-004](../decisions/adr-004-aks.md)).

## Core Components
- **Edge:** App Gateway WAF terminates TLS and routes to AKS ingress; rate limits and surge protection enforced.
- **API Gateway (in-cluster):** Validates Azure AD tokens/scopes/roles; enforces throttling.
- **Services:** Auth, Catalog, Order, Payment, Inventory, Search, Notifications—stateless .NET 8 containers with CQRS/MediatR; each owns its database.
- **Data Stores:** PostgreSQL (orders/payments), Cassandra (reviews/wishlists), Elasticsearch (product search), Redis (cache only), Kafka/Event Hubs (async backbone).
- **Secrets:** Per-region Key Vault; accessed via managed identities only.

## Azure Topology
- Hub-spoke VNets with subnets for App Gateway, AKS, and data planes.
- AKS: system/user pools, Azure CNI, AGIC/App Gateway integration, HPAs + cluster autoscaler.
- Private endpoints for data planes; no public exposure for stateful services.

## Scaling Strategy (summary)
- HPAs per service (CPU/memory/RPS/Kafka lag).
- Data: Postgres vertical + read replicas; Cassandra adds nodes; Elasticsearch adds data nodes/shards; Kafka adds brokers/partitions; Redis vertical then cluster.
- Edge: App Gateway autoscale; ingress rate limiting.

## Deployment & Environments
- Terraform: [infrastructure/global](../../infrastructure/global) (global) and [infrastructure/regions](../../infrastructure/regions) (per-region) with tfvars in [infrastructure/environments](../../infrastructure/environments).
- CI/CD: GitHub Actions runs plan/apply per region serially (eastus → westeurope) then Helm deploys; health gates before promoting next region ([docs/design/ci-cd.md](ci-cd.md)).
- Helm/K8s manifests: [deployment/kubernetes](../../deployment/kubernetes).

## Data & Consistency
- Strong: orders, payments, inventory.
- Eventual: reviews, wishlist, notifications, analytics.
- Each service owns its schema; no shared DBs ([ADR-007](../decisions/adr-007-postgresql.md), [ADR-008](../decisions/adr-008-cassandra.md)).

## Security (high level)
- Azure AD/AAD B2C tokens validated at gateway and services; scopes/roles enforced.
- TLS everywhere; secrets in Key Vault; managed identities for resource access.
- API Gateway + App Gateway WAF as defense-in-depth ([ADR-002](../decisions/adr-002-api-gateway.md)).

## Observability
- Logs/metrics/traces to Azure Monitor + Log Analytics + App Insights; Prometheus/Grafana optional in-cluster.
- Alerts/dashboards under [observability](../../observability).

## Traceability
- Requirements: [docs/requirements](../requirements).
- Decisions: [docs/decisions](../decisions).
- Scaling runbooks: [docs/design/scalability-strategy.md](scalability-strategy.md).