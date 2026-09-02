# Employee Operations

An internal equipment-request workflow demonstrating controlled transitions rather than controller-written status fields.

## Architecture

- `Domain`: lifecycle and append-only transition facts, without infrastructure dependencies.
- `Application`: validated DTOs, use cases, repository port, and conflict/not-found errors.
- `Infrastructure`: EF Core 10 SQL Server mappings and optimistic concurrency.
- `Api`: JWT bearer auth, Employee/Manager/Operations policies, Problem Details, JSON logs, and health checks.
- `client`: Angular 22 standalone screens, reactive forms, guard, interceptor, and loading/validation/error states.
- `tests`: transition matrix and in-process HTTP scenarios.

`Version` is an EF concurrency token. The service rejects stale input before mutation; EF's original-value check catches a race between read and commit. Transition rows cannot be modified or deleted through normal persistence.

## Local setup

```bash
export EMPLOYEE_OPS_SQL_PASSWORD='strong-local-password'
export EMPLOYEE_OPS_JWT_SIGNING_KEY='at-least-32-local-development-characters'
docker compose up -d sqlserver
dotnet user-secrets set --project src/EmployeeOperations.Api 'ConnectionStrings:EmployeeOperations' 'Server=localhost,1433;Database=EmployeeOperations;User Id=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True'
dotnet user-secrets set --project src/EmployeeOperations.Api 'Jwt:SigningKey' 'YOUR_SIGNING_KEY'
dotnet ef database update --project src/EmployeeOperations.Infrastructure --startup-project src/EmployeeOperations.Api
dotnet run --project src/EmployeeOperations.Api
cd client && npm ci && npm start
```

The client stores a development JWT only in session storage. Production should use the company's OIDC provider. No credentials are committed.

## Verification and deliberate failure

```bash
dotnet test EmployeeOperations.slnx -c Release
cd client && npm test -- --watch=false && npm run build
```

The HTTP suite submits and approves a request, then sends a reject with the previous version. It expects `409`; Approved state and its audit history remain intact.

## Reproducible baseline benchmark

```bash
dotnet run -c Release --project benchmarks/EmployeeOperations.Benchmarks -- --connection '...' --commit "$(git rev-parse HEAD)" --seed 1209 --size 10000 --warmups 2 --runs 10 --output benchmarks/results/baseline.json
```

The JSON records commit, runtime/machine, generator/seed, size, concurrency, warm-up, run count, median/p95, raw output, and optimization status. No performance result is claimed until a real output is committed; projection, `AsNoTracking`, pagination, and index work follow the baseline.
