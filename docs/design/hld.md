# High-Level Design (HLD)

## Overview
AmCart is a B2C e-commerce platform for clothing and accessories. It serves customers and administrators through a single SPA backed by microservices exposed via an API Gateway.

## Global vs Regional Architecture
- **Global layer** (single deployment): Azure AD / Azure AD B2C, Azure DNS or Front Door, Azure Traffic Manager (optional), Azure Container Registry, Terraform state storage, and GitHub Actions. These services coordinate identity, routing, and CI/CD but never host customer workloads.
- **Regional stacks** (per Azure region: eastus, westeurope, etc.): Application Gateway + WAF, AKS cluster, PostgreSQL primary + replicas, Cassandra cluster, Elasticsearch cluster, Kafka cluster, Redis cache, Key Vault, Log Analytics workspace, and Observability agents. Each region is a fully self-sufficient slice that can deploy, scale, or fail independently.
- **Principles**: no synchronous cross-region calls, data residency respected, failover handled via global routing, and infrastructure code/remote state isolated per region.

## Core Components
- Azure Application Gateway (WAF v2): internet ingress, TLS termination, rate limiting, and routing into AKS.
- Azure API Management / Gateway inside AKS: enforces JWT validation, throttling, and routes traffic to downstream services.
- Microservices (per bounded context: auth, catalog, cart, order, payment, inventory, review, notification, search): stateless deployments on Kubernetes.
- Data Stores: Azure Database for PostgreSQL Flexible Server (single-writer, geo-read replicas), Azure-managed Cassandra (or self-hosted on AKS), Elasticsearch (or Azure Cognitive Search) for search, Azure Cache for Redis for caching.
- Messaging: Kafka for async processing and fan-out; topics sized per service domain.
- Secrets: Azure Key Vault per service boundary, accessed via managed identities.

## Azure Topology
- Networking: Hub-spoke VNet with subnets for Application Gateway, AKS node pools, data services, and shared platform services.
- Compute: Azure Kubernetes Service (AKS) with system and user node pools, managed identity enabled, Azure CNI networking, autoscaling via HPAs and cluster autoscaler.
- Messaging: Kafka (or a Kafka-compatible managed service) provides the streaming backbone for order events, payments, search indexing, and notifications. Each region hosts its own Kafka namespace/cluster.
- Data Plane: PostgreSQL (orders/payments), Cassandra (catalog, inventory), Redis (sessions, caching), Elasticsearch/Cognitive Search (product search), blob storage for media.
- Observability: Azure Monitor, Log Analytics, Application Insights, and Prometheus/Grafana stack inside AKS.

## Scaling Strategy (summary)
- Philosophy: metrics-driven, configuration-based scaling; no code changes required.
- Application: HPAs per service; triggers include CPU, memory, RPS, and Kafka consumer lag.
- Data: PostgreSQL vertical + read replicas; Cassandra adds nodes; Elasticsearch adds data nodes/shards; Kafka adds brokers/partitions; Redis vertical then cluster.
- Edge: Ingress and gateway auto-scale; rate limiting protects backends.
- Detailed guidance: see docs/design/scalability-strategy.md and deployment/kubernetes/hpa/.

## Deployment & Environments
- Containerized services deployed to regional AKS clusters; manifests live under deployment/kubernetes/ with per-region overlays (namespaces, ingress, HPAs, Key Vault references).
- Infrastructure-as-code organized as follows:
	- infrastructure/global/ for DNS, Traffic Manager/Front Door, ACR, Azure AD artifacts.
	- infrastructure/regions/<region>/ for networking, AKS, Application Gateway, PostgreSQL, Cassandra, Elasticsearch, Kafka, Redis, Key Vault, and monitoring.
	- infrastructure/environments/ contains tfvars/backends per region (e.g., prod-eastus.tfvars, prod-westeurope.tfvars) so each region can be planned/applied independently.
	- Terraform remote state is isolated per region (key names such as `prod-eastus.tfstate`, `prod-westeurope.tfstate`).

## Data & Consistency
- Strong consistency for orders, payments, inventory.
- Eventual consistency acceptable for reviews, wishlists, notifications, analytics.
- Each service owns its schema; no shared databases between services.

## Security (high level)
- Identity: Azure Active Directory (Microsoft Entra ID) issues tokens for administrators and CI/CD; Azure AD B2C handles customer identities. Tokens follow OAuth2/OIDC (Authorization Code with PKCE for SPA) and are validated at the API Gateway before reaching services.
- Authorization: API Gateway enforces scopes and roles from Azure AD app registrations; services honor `roles`/`scp` claims for fine-grained checks and leverage managed identities for service-to-service calls.
- Secrets & trust: All traffic over HTTPS with TLS termination at Azure Application Gateway (WAF). Secrets, certificates, and connection strings live in Azure Key Vault, accessed via managed identities—no passwords in cookies or source control.

## Observability
- Centralized logging; metrics and tracing emitted by every service.
- Alerts defined in observability/alerts/; dashboards in observability/dashboards/; metrics definitions in observability/metrics/.

## Traceability
- Requirements: docs/requirements/functional-requirements.md and docs/requirements/non-functional-requirements.md.
- Decisions: docs/decisions/ for ADRs.
- Scaling rules and operational runbooks: docs/design/scalability-strategy.md and deployment/kubernetes/.