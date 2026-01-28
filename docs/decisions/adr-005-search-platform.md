# ADR-005: Search Platform (Elasticsearch)

- Status: Accepted
- Date: 2026-01-28

## Context
AmCart needs fast, flexible, and scalable search capabilities, including full-text search, filtering, sorting, and faceted navigation across product catalogs. Relational databases are not optimized for these workloads at scale.

## Decision
Adopt Elasticsearch as the dedicated search and indexing platform. Elasticsearch maintains a denormalized, read-optimized index of product and catalog data consumed by the SPA and APIs.

## Rationale
- Purpose-built for full-text search, aggregations, and relevance tuning.
- Supports faceted filtering and sorting required by the storefront.
- Provides near-real-time indexing and horizontal scalability.
- Decouples read-heavy search workloads from transactional databases.

## Consequences
### Positive
- Significantly improved search performance and user experience.
- Reduced load on PostgreSQL/Cassandra.
- Enables advanced discovery experiences and analytics.

### Negative
- Data duplication and potential staleness between source-of-truth databases and search index.
- Additional infrastructure component to manage.

## Mitigations
- Populate indices asynchronously via Kafka events and scheduled backfills.
- Treat Elasticsearch strictly as a derived read model; no transactional writes.
- Implement monitoring and re-indexing runbooks.

## Notes
Elasticsearch is not a system of record and must never be used for transactional updates.
