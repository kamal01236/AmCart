# Kafka Topic Sizing & Alert Thresholds

## Topic Sizing (baseline)
- Partitions: size for peak throughput + 30% headroom. Target 50–100 MB/s per broker aggregate; 5–10 MB/s per partition is typically safe.
- Replication factor: 3 for production; 2 for non-prod. Keep ISR >= 2 at all times in prod.
- Message size: prefer <1 MB. If larger, use compression and consider payload offloading.
- Retention: align with business SLAs; default 7d for transactional streams; use compaction for idempotent facts.

## Alert Thresholds (Prometheus rules guidance)
- Consumer lag (per consumer group, per partition):
  - Warning: lag > 5,000 messages for 5m
  - Critical: lag > 20,000 messages for 5m
- Broker health:
  - ISR shrinkage: under-replicated partitions > 0 for 5m
  - Offline partitions: > 0 immediately critical
- Disk utilization per broker:
  - Warning: > 70%
  - Critical: > 80%
- Controller changes: > 3 changes in 10m (flapping)
- Request latency (p99 produce/fetch):
  - Warning: > 500 ms for 5m
  - Critical: > 1 s for 5m

## Scaling Actions
- Add partitions: when sustained consumer lag is driven by throughput (not consumer failures).
- Add brokers: when disk or network approaches thresholds or partitions per broker exceed target.
- Increase consumer replicas: tie HPA to consumer lag metric to scale pods.

## Runbook Pointers
- Partition change requires client restart/rebalance awareness; ensure consumer groups handle new partitions.
- After scaling, validate ISR recovery and reassignment completion.
- Coordinate retention changes with storage and backup policies.
