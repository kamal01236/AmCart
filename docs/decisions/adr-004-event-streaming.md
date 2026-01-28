# ADR-004: Event Streaming Platform (Kafka)

- Status: Accepted
- Date: 2026-01-28

## Context
AmCart requires an asynchronous, event-driven communication mechanism to decouple microservices and support high-throughput processing for events such as order placement, payment completion, inventory updates, notifications, audit logging, and search indexing. The platform must support horizontal scalability, fault tolerance, and consumer group semantics.

## Decision
Adopt Apache Kafka (or a Kafka-compatible managed service) as the enterprise event streaming platform. Services publish and consume domain events using Kafka client libraries and consumer groups, with topics aligned to bounded contexts.

## Rationale
- Enables loose coupling between producer and consumer services.
- Provides high-throughput, low-latency event delivery.
- Durable, replayable event logs supporting backfills and audits.
- Aligns with event-driven architecture patterns and industry standards.

## Consequences
### Positive
- Improved scalability and resilience via asynchronous workflows.
- Reduces synchronous dependencies across services.
- Unlocks analytics, audit, and integration use cases through event replay.

### Negative
- Requires event schema governance and versioning discipline.
- Introduces eventual consistency across services.
- Operational complexity if Kafka is self-managed.

## Mitigations
- Enforce schema versioning and backward-compatible events.
- Ensure consumers are idempotent and resilient to duplicates.
- Prefer managed Kafka or Kafka-compatible services when possible.

## Notes
Kafka transports business events only; it is not used for synchronous command execution.
