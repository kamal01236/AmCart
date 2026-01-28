# ADR-007: Relational Database Platform (PostgreSQL)

- Status: Accepted
- Date: 2026-01-28

## Context
AmCart requires a reliable, strongly consistent data store for core transactional and master data domains—orders, payments, inventory, pricing, product definitions, and similar records. These workloads demand ACID guarantees, referential integrity, predictable transactions, and enterprise-grade availability, backup, and security. The platform must scale reads, support point-in-time recovery, and integrate cleanly with Azure services.

## Decision
Adopt PostgreSQL as the primary relational database system of record for all transactional and strongly consistent data. PostgreSQL runs as an Azure managed database service (single-writer with multiple read replicas) inside private networking. Each microservice owns its schema; there are no shared databases.

## Rationale
- Full ACID compliance ensures correctness for financial and order operations.
- Mature SQL feature set (joins, constraints, stored procedures) required by transactional workloads.
- Widely adopted open-source database with strong ecosystem support.
- Managed Azure offerings supply HA, backups, monitoring, and easy scaling.
- Fits single-writer/multi-reader patterns common to e-commerce platforms.

## Consequences
### Positive
- Strong consistency for orders, payments, and inventory.
- Clear system of record supporting audits and compliance.
- Read scalability via replicas, predictable transactional semantics.

### Negative
- Write scalability limited to the primary node.
- Requires careful schema/index design and tuning.
- Horizontal sharding is non-trivial.

## Mitigations
- Offload read-heavy traffic to replicas and Redis cache.
- Keep write transactions small and optimized; plan vertical scaling for peaks.
- Use Kafka to decouple downstream processing and avoid cross-service DB access.
- Enforce private networking and store credentials in Azure Key Vault.

## Deployment & Topology
- Primary writer plus read replicas (Azure Database for PostgreSQL Flexible Server).
- Automated backups with point-in-time recovery.
- Private endpoints only; no public ingress.
- Secrets managed via Azure Key Vault and retrieved with managed identities.

## Notes & Constraints
- PostgreSQL is only used by the service that owns the data; no shared schemas.
- Not used for search (handled by Elasticsearch), event streaming (Kafka), or eventually consistent high-volume data (Cassandra).
- Multi-region writes are avoided to preserve consistency; cross-region reads handled via replicas.

## Alignment with Enterprise Principles
- **Data Integrity First:** ACID guarantees for critical data.
- **Separation of Concerns:** Dedicated search/cache/event platforms handle specialized workloads.
- **Scalability by Design:** Read replicas and event-driven offloading relieve the primary.
- **Security by Default:** Encrypted connections, least-privilege access, Key Vault secret storage.
