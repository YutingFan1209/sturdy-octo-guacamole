# Analytics & Reporting

An API and independent worker for large operational CSV files. Uploads use a fixed 64 KiB buffer and sequential async I/O. SQL Server stores durable job state, 500-row checkpoints, idempotent `(JobId, LineNumber)` records, and report projections.

The worker leases Queued or Processing jobs. On restart it skips the stored checkpoint, repeats an uncheckpointed batch safely, and continues. Invalid rows fail with a diagnostic.

```bash
dotnet restore AnalyticsReporting.slnx
dotnet test AnalyticsReporting.slnx -c Release
```

Configure `ConnectionStrings:Analytics`, `Storage:Root`, and JWT settings outside source control. The failure test interrupts after line 500 and resumes to 1,200 unique rows. The bounded-I/O test streams 5 MiB while asserting no read request exceeds 64 KiB. SQL report p50/p95 before/after optimization remains unclaimed until raw results are committed.
