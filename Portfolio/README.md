# Enterprise Systems Portfolio

These are independently deployable systems, not a shared CRUD framework. Employee Operations is the first runnable vertical slice; the other systems are deliberately not empty templates.

## Evidence bank

Each project has three qualitative bullets and exactly two quantified bullets. Unmeasured numbers remain targets.

### Employee Operations
- Built Angular and REST equipment-request creation, submission, manager decision, and operations completion.
- Kept lifecycle and audit rules in the aggregate, with application use cases coordinating persistence.
- Added role policies, append-only history protection, Problem Details, and optimistic concurrency conflicts.
- **Target:** measure median/p95 dashboard latency on at least 10,000 deterministic requests; no result is claimed yet.
- **Measured:** tests cover all 6 allowed lifecycle edges and 19 forbidden edges, plus HTTP authorization and stale-version failure.

### Asset & Access Manager
- Built the MVC/Razor asset-search and laptop-assignment vertical slice.
- Uses conditional SQL reservation plus a filtered unique index for active assignments.
- Added administrator policy protection, transactional persistence, and clear collision feedback.
- **Target:** measure search latency across at least 25,000 seeded assets.
- **Measured:** 32 simultaneous attempts produced 1 assignment, 31 explicit conflicts, and 0 undetected collisions in the automated contention test.

### Compliance Case Management
- Planned modular-monolith investigation and escalation workflows.
- Planned explicit transitions, policy authorization, and module-owned audit history.
- Planned analyst/worker concurrency coordination.
- **Target:** measure deadline evaluation throughput and delay.
- **Target:** report scoped domain/application test count and coverage.

### Analytics & Reporting
- Planned streaming upload, queued validation, resumable jobs, status, and reporting APIs.
- Planned bounded-memory pipelines and idempotent checkpoints.
- Planned safe restart after worker interruption.
- **Target:** measure maximum file size and peak memory before/after streaming.
- **Target:** measure reporting p50/p95 before/after optimization.

### Reconciliation Platform
- Planned ingestion, matching, exception, and notification services.
- Planned inbox/outbox boundaries and trace propagation.
- Planned dead-letter visibility, retries, and idempotent exception creation.
- **Target:** measure record-processing throughput under documented concurrency.
- **Target:** prove duplicate deliveries produce 0 duplicate side effects.
