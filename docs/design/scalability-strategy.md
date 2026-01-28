# Scalability Strategy

## Philosophy
- Scale at the right layer, driven by metrics, via configuration (never code changes).
- Primary controls: Kubernetes HPAs, platform/service configs (DB/Kafka/ES/Redis), autoscaling rules, CI/CD promotion.

## Layer-by-Layer Approach
- Application: Horizontal scaling via HPAs per service; scale triggers from CPU, memory, request rate, Kafka consumer lag.
- API Gateway/Ingress: Managed autoscale; enforce rate limits and surge protection.
- Databases: PostgreSQL vertical scale for write pressure; horizontal with read replicas; single-writer, multi-reader in multi-region.
- Cassandra: Horizontal node add; automatic rebalancing.
- Elasticsearch: Add data nodes and shards; rebalance.
- Kafka: Add brokers; increase partitions; consumers scale with pods.
- Redis: Vertical first; Redis Cluster if needed; eviction tuned to protect memory.

## Decision Flow (runtime)
Traffic ↑ → Ingress scales → HPAs add pods → Cache absorbs reads → Read replicas handle DB reads → Kafka partitions handle async load.

## Configuration Locations
- HPAs: deployment/kubernetes/hpa/
- Ingress/rate limit: deployment/kubernetes/ingress/
- PostgreSQL: infrastructure/regions/<region>/postgres/
- Cassandra: infrastructure/regions/<region>/cassandra/
- Elasticsearch: infrastructure/regions/<region>/elasticsearch/
- Kafka: infrastructure/regions/<region>/kafka/
- Redis: infrastructure/regions/<region>/redis/
- Alerts/dashboards: observability/alerts/, observability/dashboards/, observability/metrics/

## Capacity & Scaling Checklist (operational)
- HPAs: min/max replicas set; metrics defined (cpu, memory, RPS, consumer lag); scale-to-zero avoided for stateful flows.
- Ingress: rate limits and surge controls defined; upstream timeouts set; 429 behavior documented.
- PostgreSQL: max connections sized; read replicas count/region documented; failover plan tested; backups and PITR verified.
- Cassandra: replication factor set; node count headroom ≥15%; disk usage alarm <70%.
- Elasticsearch: shard/replica plan per index; heap <75%; ILM/retention set; snapshot schedule defined.
- Kafka: partitions sized for target throughput; retention and compaction set; ISR alerting configured; consumer lag alerts wired to HPAs.
- Redis: maxmemory and eviction policy set; hit ratio tracked; persistence choice documented.
- CI/CD: scaling configs versioned; rollout/rollback steps documented; config changes require review.
- Observability: SLOs defined; dashboards linked in runbooks; alerts have clear owners and actions.

## References
- HPA template: deployment/kubernetes/hpa/hpa-template.yaml
- Kafka sizing & alerts: observability/alerts/kafka-scaling-alerts.md
- HLD scaling summary: docs/design/hld.md
- LLD runtime configs: docs/design/lld.md
