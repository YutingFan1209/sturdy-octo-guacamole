# Compliance Case Management

A modular-monolith slice for controlled investigations. The `Cases` module owns transitions and immutable audit facts; the API applies analyst policy authorization; EF Core provides SQL Server persistence and concurrency checks; a separate host runs scheduled escalation.

Lifecycle: `Open → UnderInvestigation → Resolved → Closed`, while overdue Open or UnderInvestigation cases may become Escalated and then Resolved. Controllers and the worker invoke domain methods rather than setting status.

The required race is automated: two analyst transitions and one deadline escalation start together from version 0. An atomic compare/exchange persistence boundary accepts exactly one, rejects two stale writes, and preserves the winner's complete two-entry audit history. EF maps `Version` as a concurrency token and refuses normal audit mutation/deletion.

```bash
dotnet restore ComplianceCaseManagement.slnx
dotnet test ComplianceCaseManagement.slnx -c Release
```

Configure `ConnectionStrings:ComplianceCases` and `Jwt:SigningKey` through user secrets or environment variables. No secrets are committed. The benchmark harness requires commit SHA and records dataset generator/seed, machine/runtime, warm-up, run count, raw timings, median and p95. Persistent scheduling delay remains a target until SQL Server is measured.
