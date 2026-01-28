# ADR-008: Distributed NoSQL Database Platform (Apache Cassandra)

- Status: Accepted
- Date: 2026-01-28

## Context
AmCart needs a highly available, horizontally scalable data store for high-volume, write-intensive, and eventually consistent workloads—wishlists, reviews, notifications metadata, user activity logs, and event-derived read models. These domains require low-latency reads/writes, tolerate partial failures, and do not need strict transactional guarantees. Relational databases are poorly suited to these access patterns.

## Decision
Adopt Apache Cassandra as the distributed NoSQL database for eventually consistent, availability-critical domains. Each region operates a multi-node cluster with quorum-based reads/writes (RF=3, LOCAL_QUORUM) and private networking. Cassandra complements PostgreSQL by handling workloads where availability outweighs strict consistency.

## Rationale
- Near-linear horizontal scalability by adding nodes.
- Masterless architecture delivers high availability and fault tolerance.
- Tunable consistency supports eventual consistency scenarios.
- Optimized for write-heavy workloads and large data volumes.
- Proven in global, large-scale systems.

## Consequences
### Positive
- Predictable performance and low latency at scale.
- No downtime during node failures or maintenance windows.
- Resilient operations across regions.

### Negative
- Eventual consistency and limited querying capabilities.
- Requires careful data modeling and operational expertise.

## Mitigations
- Restrict Cassandra to domains tolerant of eventual consistency.
- Design tables around query patterns; enforce schema versioning.
- Ensure producers/consumers use idempotent writes and handle duplicates.
- Feed Cassandra via Kafka to decouple ingestion and downstream reads.
- Monitor latency/compaction and rebalance proactively; maintain snapshots/offsite backups.

## Deployment & Topology
- Minimum 3 nodes per region; replication factor 3.
- Consistency levels: LOCAL_QUORUM for reads/writes.
- Private endpoints only; no public exposure.
- Regular snapshots and offsite backups.

## Notes & Constraints
- Cassandra is not a system of record for financial or order data.
- Not used for search, payment processing, or strongly consistent operations.
- Cross-region writes avoided; regions operate independently, with acceptable data duplication.

## Alignment with Enterprise Principles
- **Availability over Consistency:** Keeps the platform responsive under failure.
- **Right Tool for the Job:** Complements PostgreSQL for workloads where relational guarantees aren’t required.
- **Scalability by Design:** Horizontal expansion without re-architecture.
- **Failure Tolerance:** Built-in resilience to node and rack failures.

**Summary**
Apache Cassandra serves high-volume, availability-critical, eventually consistent domains, while PostgreSQL remains the authoritative store for transactional data. The split ensures each workload uses the most appropriate persistence technology.
