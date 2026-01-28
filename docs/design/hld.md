# High-Level Design (HLD)

## Overview
AmCart is a B2C e-commerce platform for clothing and accessories. It serves customers and administrators through a single SPA backed by microservices exposed via an API Gateway.

## Core Components
- API Gateway/Ingress: routes traffic, enforces TLS, rate limits, and auth integration.
- Microservices (per bounded context: auth, catalog, cart, order, payment, inventory, review, notification, search): stateless deployments on Kubernetes.
- Data Stores: PostgreSQL for transactional data (single-writer, read replicas); Cassandra for wide-column needs; Elasticsearch for search; Redis as cache only.
- Messaging: Kafka for async processing and fan-out; topics sized per service domain.

## Scaling Strategy (summary)
- Philosophy: metrics-driven, configuration-based scaling; no code changes required.
- Application: HPAs per service; triggers include CPU, memory, RPS, and Kafka consumer lag.
- Data: PostgreSQL vertical + read replicas; Cassandra adds nodes; Elasticsearch adds data nodes/shards; Kafka adds brokers/partitions; Redis vertical then cluster.
- Edge: Ingress and gateway auto-scale; rate limiting protects backends.
- Detailed guidance: see docs/design/scalability-strategy.md and deployment/kubernetes/hpa/.

## Deployment & Environments
- Containerized services deployed to Kubernetes; manifests live under deployment/kubernetes/ (hpa, ingress, namespaces).
- Infrastructure-as-code under infrastructure/regions/<region>/ for databases, Kafka, Elasticsearch, Redis.

## Data & Consistency
- Strong consistency for orders, payments, inventory.
- Eventual consistency acceptable for reviews, wishlists, notifications, analytics.
- Each service owns its schema; no shared databases between services.

## Security (high level)
- Auth via OAuth2/OIDC (Auth0); API Gateway performs authZ enforcement; services validate tokens and roles.
- All traffic over HTTPS; secrets managed via platform secret store; no passwords in cookies.

## Observability
- Centralized logging; metrics and tracing emitted by every service.
- Alerts defined in observability/alerts/; dashboards in observability/dashboards/; metrics definitions in observability/metrics/.

## Traceability
- Requirements: docs/requirements/functional-requirements.md and docs/requirements/non-functional-requirements.md.
- Decisions: docs/decisions/ for ADRs.
- Scaling rules and operational runbooks: docs/design/scalability-strategy.md and deployment/kubernetes/.