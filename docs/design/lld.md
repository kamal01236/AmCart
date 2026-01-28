# Low-Level Design (LLD)

## Runtime Configuration Artifacts
- Kubernetes manifests: deployment/kubernetes/ (deployments, services, ingress, hpa, namespaces).
- HPAs: deployment/kubernetes/hpa/ (see hpa-template.yaml for defaults: cpu 70%, memory 75%, RPS and Kafka lag examples).
- Ingress and rate limiting: deployment/kubernetes/ingress/.

## Identity & Access Configuration
- Azure AD app registrations (SPA + APIs) and Azure AD B2C tenant settings are managed via [infrastructure/global/aad/main.tf](infrastructure/global/aad/main.tf), which composes modules under infrastructure/modules/identity/ and is parameterized through environment overlays (e.g., [infrastructure/environments/dev-global.tfvars](infrastructure/environments/dev-global.tfvars)).
- SPA uses Authorization Code + PKCE against Azure AD/AD B2C; configuration (client IDs, scopes, redirect URIs, authority) is injected via Angular environment files sourced from Key Vault secrets named `aad-*-<env>` that Terraform can populate automatically.
- AKS workloads authenticate to Azure resources (Key Vault, storage, databases) using managed identities defined per namespace; the per-region Terraform at [infrastructure/regions/eastus/identity/main.tf](infrastructure/regions/eastus/identity/main.tf) and [infrastructure/regions/westeurope/identity/main.tf](infrastructure/regions/westeurope/identity/main.tf) creates `mi-amcart-<service>-<env>-<region>` identities. GitHub Actions uses OIDC federation to obtain short-lived Azure tokens for deployments through the `amcart-ci` app registration.

## Azure Infrastructure Components
- Terraform regional stacks live under infrastructure/regions/eastus and infrastructure/regions/westeurope, each provisioning hub-and-spoke networking, AKS (managed identity, Azure CNI, AGIC/App Gateway), Azure Database for PostgreSQL Flexible Server, Azure Cache for Redis, Azure Event Hubs for Kafka-compatible messaging, Key Vault per domain, and paired Log Analytics + Application Insights.
- Environment overlays use infrastructure/environments/<env>-<region>.tfvars (dev/staging/prod × eastus/westeurope) and dedicated remote state per region to keep blast radius isolated; CI/CD applies tfvars explicitly to avoid cross-region coupling.
- Modules under infrastructure/modules encapsulate common patterns: `networking` (VNets/subnets/NSGs), `identity` (managed identities, AD app registrations, federated credentials), `data` (Postgres/Cassandra/Redis), and `monitoring` (Log Analytics, Diagnostics).

## Multi-Region Deployment Model
- Active-active strategy: eastus serves as primary for 70% of load, westeurope handles the remaining 30% plus provides disaster recovery capacity; Azure Traffic Manager steers users while App Gateway WAF instances terminate TLS per region.
- Regional stacks operate autonomously—no shared resource groups or service instances—so failover means shifting Traffic Manager weights instead of recreating infrastructure; data stores rely on native replication (PostgreSQL read replicas, Cassandra multi-region replication, Kafka mirroring) configured in each regional folder.
- Deployment sequencing: GitHub Actions plans/applies Terraform per region serially (eastus → westeurope) and only promotes an application release to westeurope after eastus health checks succeed; Helm releases use the same ordering enforced via required workflow jobs.

## Service Implementation Blueprint
- Each service folder (services/<service>-service) hosts an ASP.NET Core solution with API layer, application/domain layer (CQRS + MediatR), EF Core DbContext with migrations, and messaging adapters for Azure Event Hubs.
- API contracts live in services/<service>-service/api/openapi.yaml and drive controller generation + contract tests.
- Database artifacts (schema.sql, seed data) describe initial tables and static reference data; migrations kept alongside source.
- Dockerfiles target .NET 8, multi-stage build (restore → publish → runtime) and include health probes and non-root execution.
- Helm charts define deployments, services, autoscaling, configmaps, pod identity bindings, and Key Vault CSI secret references.
- Tests: unit tests (xUnit + FluentAssertions) and integration tests (TestContainers for Postgres/Redis/Event Hubs) live in services/<service>-service/tests.

## Data Layer Details
- PostgreSQL: infrastructure/regions/<region>/postgres/ (primary.tf, read-replicas.tf, parameters.tf); single-writer, read replicas for scale; connection limits sized per service.
- Cassandra: infrastructure/regions/<region>/cassandra/ (cluster.tf, replication.yaml, capacity.md); replication factor and node counts defined per keyspace.
- Elasticsearch: infrastructure/regions/<region>/elasticsearch/ (cluster.tf, index-settings.json, scaling.md); shard/replica settings per index.
- Redis: infrastructure/regions/<region>/redis/ (redis.tf, eviction-policy.conf); maxmemory and eviction policy documented.

## Messaging Details
- Kafka: infrastructure/regions/<region>/kafka/ (cluster.tf, topics.yaml, capacity.md); partitions and retention per topic; replication factor defaults to 3 in prod.
- Observability rules for Kafka scaling and alerts live in observability/alerts/kafka-scaling-alerts.md.

## Service Contracts
- Each microservice owns its API and schema; APIs documented via OpenAPI in the respective service repo folder under services/<service>/.
- Async contracts (Kafka topics, events) documented alongside producer/consumer services in services/<service>/README.md.

## Operational Runbooks
- Scaling and capacity checklist: docs/design/scalability-strategy.md.
- Alerting and dashboards: observability/alerts/, observability/dashboards/, observability/metrics/.