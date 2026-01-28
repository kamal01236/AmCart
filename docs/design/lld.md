# Low-Level Design (LLD)

## Runtime Configuration Artifacts
- Kubernetes manifests: deployment/kubernetes/ (deployments, services, ingress, hpa, namespaces).
- HPAs: deployment/kubernetes/hpa/ (see hpa-template.yaml for defaults: cpu 70%, memory 75%, RPS and Kafka lag examples).
- Ingress and rate limiting: deployment/kubernetes/ingress/.

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