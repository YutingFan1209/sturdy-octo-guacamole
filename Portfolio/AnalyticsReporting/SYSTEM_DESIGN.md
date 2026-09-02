# Analytics & Reporting System Design

```mermaid
flowchart LR
  U[Reporting user] --> A[Upload and report API]
  A --> F[(Raw file store)]
  A --> D[(Analytics SQL)]
  W[Processing worker] --> D
  W --> F
  D --> A
```

The file store owns immutable uploads. SQL owns job state, checkpoints, validated rows, and report indexes. Committing a Queued job is the durable queue boundary.

```mermaid
sequenceDiagram
  participant Client
  participant API
  participant Files
  participant SQL
  participant Worker
  Client->>API: multipart CSV stream
  API->>Files: bounded copy
  API->>SQL: Queued job
  Worker->>SQL: lease Queued/Processing
  Worker->>Files: read after checkpoint
  loop every 500 rows
    Worker->>SQL: idempotent batch
    Worker->>SQL: checkpoint
  end
  Worker->>SQL: Completed
```

Lifecycle is `Queued → Processing → Completed|Failed`. A crash before checkpoint repeats at most one batch; the unique job/line key suppresses duplicate effects. A crash after checkpoint resumes at the next line. Malformed CSV records the failing line. File-copy failure precedes job creation, preventing dangling jobs.
