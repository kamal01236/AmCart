# ADR-006: Caching Strategy (Redis)

- Status: Accepted
- Date: 2026-01-28

## Context
AmCart experiences high read traffic for frequently accessed data such as product details, pricing snapshots, inventory availability, and user session metadata. Serving every request from databases would increase latency and operational cost.

## Decision
Adopt Redis as the in-memory caching layer to improve performance and reduce load on backend databases and services. Redis instances are deployed per environment with clear namespace conventions and TTL policies.

## Rationale
- Extremely low-latency reads/writes suitable for hot data.
- Simple key-value model with built-in TTL eviction.
- Cloud-native managed offerings available across Azure regions.
- Proven track record for session storage, caching, and rate limiting.

## Consequences
### Positive
- Improved response times for the SPA and APIs.
- Reduced pressure on PostgreSQL/Cassandra and downstream services.
- Better scalability during peak demand (sales, promotions).

### Negative
- Cache invalidation complexity and risk of serving stale data.
- Data loss possible on node restart or eviction.

## Mitigations
- Treat Redis strictly as a non-authoritative cache; never store system-of-record data.
- Use TTLs plus event-driven invalidation (Kafka events) to keep cache entries fresh.
- Ensure application correctness does not rely on cache availability (fallback to source of truth).

## Notes
Redis usage spans response caching, session tokens, feature flags, and rate limiting; all patterns must document eviction strategies and hit/miss metrics.
