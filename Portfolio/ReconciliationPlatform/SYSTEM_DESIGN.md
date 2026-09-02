# Reconciliation Platform System Design

## Architecture and data ownership

```mermaid
flowchart LR
  S[Business systems] --> I[Ingestion API]
  I --> Q[(Inbox SQL)]
  R[Reconciliation worker] --> Q
  R --> C[(Exception cases)]
  R --> O[(Notification outbox)]
  N[Notification worker] --> O
  N --> P[Notification provider]
  N --> D[(Dead letters)]
  A[Operations API] --> D
```

Ingestion owns validation and transport identity. The reconciliation service owns business identity and exception cases. Notification owns delivery attempts. SQL constraints are final idempotency boundaries; workers are independently deployable and share only contracts and persistence schemas.

## Duplicate and outage sequence

```mermaid
sequenceDiagram
  participant Source
  participant Ingest
  participant Inbox
  participant Reconciler
  participant Outbox
  participant Notify
  participant DLQ
  Source->>Ingest: mismatch MessageId M, trace T
  Ingest->>Inbox: insert M
  Source->>Ingest: redeliver M
  Ingest-->>Source: duplicate acknowledged
  Reconciler->>Inbox: consume M
  Reconciler->>Outbox: transactionally create case + notification
  loop three unavailable attempts
    Notify->>Outbox: lease notification
    Notify--xNotify: provider unavailable
  end
  Notify->>DLQ: message, error, trace T
```

## Lifecycle and failures

Inbox: `Received → Processed`. Notification: `Pending → Sent` or retry until `DeadLettered`. Raw payload and correlation survive each boundary. A crash before the reconciliation transaction commits leaves the inbox pending; after commit, both case and notification exist. A notification crash before Sent may redeliver, so production providers should also receive the notification ID as an idempotency key. Operators inspect `/api/reconciliation/dead-letters`, repair downstream dependencies, and replay deliberately rather than silently discarding messages.
