# Reconciliation Platform

An event-driven .NET platform with separate ingestion API, reconciliation worker, and notification worker processes. SQL Server is the durable inbox/outbox transport for this portfolio deployment.

Transport duplicates are suppressed by inbox `MessageId`. Business duplicates are suppressed by unique `(SourceSystem, RecordKey)` exception cases. Case and notification creation share a transaction. Notifications retry with exponential delay and move to a queryable dead-letter table after three failed attempts. Correlation IDs flow from ingestion through cases, notifications, logs, and dead letters.

```bash
dotnet restore ReconciliationPlatform.slnx
dotnet test ReconciliationPlatform.slnx -c Release
```

The required failure test delivers the same mismatch twice while the gateway is unavailable. It proves one exception case, zero notification side effects, three explicit failures, and one correlated dead letter. A second test proves different transport IDs for one business record still create one case.

The throughput harness requires commit SHA and records environment, generator/seed, dataset size, concurrency, warm-up, measured runs, median/p95, and raw output. No production throughput is claimed before a committed SQL-backed run.
