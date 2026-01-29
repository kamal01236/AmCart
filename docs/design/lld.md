# Low-Level Design (LLD)

## Runtime Configuration Artifacts
- Kubernetes: [deployment/kubernetes](../../deployment/kubernetes) (deployments, services, ingress, HPA, network policies, namespaces).
- Helm values per env/region in [deployment/helm-charts](../../deployment/helm-charts).
- Secrets via Key Vault CSI + managed identity per namespace.

## Identity & Access
- App registrations via [infrastructure/global/aad/main.tf](../../infrastructure/global/aad/main.tf); scopes/roles from env tfvars (e.g., [infrastructure/environments/dev-global.tfvars](../../infrastructure/environments/dev-global.tfvars)).
- Managed identities per service/region: [infrastructure/regions/eastus/identity](../../infrastructure/regions/eastus/identity) and [infrastructure/regions/westeurope/identity](../../infrastructure/regions/westeurope/identity).
- Gateway validates Azure AD tokens; services use Microsoft.Identity.Web and honor `roles`/`scp`.

## Azure Infrastructure Components
- Regional stacks: networking, AKS, App Gateway, PostgreSQL, Cassandra, Elasticsearch, Kafka, Redis, Key Vault, Log Analytics/App Insights under [infrastructure/regions](../../infrastructure/regions).
- Modules: [infrastructure/modules](../../infrastructure/modules) encapsulate networking, identity, data, and monitoring.

## Multi-Region Deployment Model
- Active-active: Traffic Manager/Front Door steers users to regional App Gateways; eastus primary (70%), westeurope secondary (30%) by default.
- Serial apply: Terraform per region with isolated remote state; Helm per region after infra health checks ([docs/design/ci-cd.md](ci-cd.md)).
- Failover: adjust Traffic Manager weights; no cross-region synchronous calls.

## Service Implementation Blueprint
- Structure: services/<service>-service with API, application/domain (CQRS + MediatR), EF Core DbContext/migrations, messaging adapters (Kafka/Event Hubs), OpenAPI at services/<service>-service/api/openapi.yaml, schema/seed under db/.
- Container: .NET 8 multi-stage Dockerfile; non-root; health probes.
- Helm: deployments, services, HPAs, configmaps, secrets/Key Vault CSI, pod identity.

## Data Layer Details
- PostgreSQL: single writer, read replicas; connection limits per service.
- Cassandra: RF=3, LOCAL_QUORUM; regional clusters.
- Elasticsearch: shard/replica per index; ILM/retention.
- Redis: cache-only; eviction policy documented.
- Kafka: partitions/retention per topic; replication factor ≥3 in prod; consumer lag drives HPAs.
- Locations per region: infrastructure/regions/<region>/(postgres|cassandra|elasticsearch|redis|kafka).

## Messaging Details
- Kafka/Event Hubs topics defined per service; contracts in service READMEs.
- Producers/consumers idempotent; retry/backoff via Polly.

## Operational Runbooks
- Scaling: [docs/design/scalability-strategy.md](scalability-strategy.md).
- Alerts/dashboards: [observability/alerts](../../observability/alerts), [observability/dashboards](../../observability/dashboards), [observability/metrics](../../observability/metrics).
- DR: adjust Traffic Manager, validate health probes, promote secondary region.

## Service Contracts
- OpenAPI per service under services/<service>-service/api/openapi.yaml.
- Async events documented in services/<service>-service/README.md.