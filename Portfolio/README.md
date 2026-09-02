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
- Built a modular-monolith investigation API and scheduled escalation worker.
- The Cases module owns explicit transitions and append-only audit history behind an EF concurrency token.
- Added analyst policy authorization and conflict-safe analyst/worker coordination.
- **Target:** measure deadline evaluation throughput and delay.
- **Measured:** 4 focused tests cover lifecycle, invalid transition, EF protections, and a 3-writer analyst/worker race; CI collects scoped coverage artifacts.

### Analytics & Reporting
- Built streaming upload, durable job status, resumable CSV processing, and category reporting.
- Uses bounded sequential I/O, 500-row checkpoints, and idempotent job/line persistence.
- Added bearer policy authorization and safe worker restart after interruption.
- **Measured:** a 5 MiB automated upload completed with every requested read buffer at or below 64 KiB; peak-memory before/after remains a benchmark target.
- **Target:** measure reporting p50/p95 before/after optimization.

### Reconciliation Platform
- Built separate ingestion, reconciliation, and notification processes.
- Uses durable inbox/outbox boundaries, business-key uniqueness, and trace propagation.
- Added exponential retries, operator-visible dead letters, and idempotent exception creation.
- **Target:** measure record-processing throughput under documented concurrency.
- **Measured:** 2 duplicate deliveries produced 1 exception case and 0 duplicate side effects; 3 simulated notification failures produced 1 correlated dead letter.
