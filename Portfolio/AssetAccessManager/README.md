# Asset & Access Manager

Traditional ASP.NET Core MVC/Razor inventory management. The first vertical slice searches laptops, atomically assigns one to an employee, and records a durable assignment.

## Correctness design

`SqlAssetStore.TryAssignAsync` performs a conditional `UPDATE ... WHERE Status = Available` inside a transaction. Only the caller that changes one row may create the assignment. A filtered unique index on `Assignments.AssetId WHERE ReturnedAt IS NULL` is defense in depth. Controllers never write status.

Cookie authentication protects the UI with an `AssetAdministrator` policy. A development-only sign-in simulates enterprise SSO; production must replace it with the organization's OIDC provider.

## Run

```bash
export ASSET_ACCESS_SQL_PASSWORD='strong-local-password'
docker compose up -d
dotnet user-secrets set --project src/AssetAccessManager.Web 'ConnectionStrings:AssetAccess' 'Server=localhost,1434;Database=AssetAccessManager;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True'
dotnet tool restore
dotnet ef database update --project src/AssetAccessManager.Infrastructure --startup-project src/AssetAccessManager.Web
dotnet run --project src/AssetAccessManager.Web
```

## Verification and measured failure scenario

The automated contention test releases 32 administrator tasks simultaneously. Exactly 1 assignment succeeds, 31 receive `AssetAlreadyAssignedException`, and the repository records 1 side effect (0 undetected collisions).

```bash
dotnet test AssetAccessManager.slnx -c Release
```

## Search benchmark

No latency is claimed yet. The harness requires commit SHA, deterministically seeds at least 25,000 assets, and records environment, seed, size, warm-up, raw runs, median, and p95.

```bash
dotnet run -c Release --project benchmarks/AssetAccessManager.Benchmarks -- --connection '...' --commit "$(git rev-parse HEAD)" --seed 1209 --size 25000 --warmups 2 --runs 10 --term Model-250 --output benchmarks/results/baseline.json
```
