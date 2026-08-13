# MovieShop Azure deployment

The first production deployment uses:

- Angular: Azure Static Web Apps
- MVC/Razor frontend: Azure App Service (.NET 10)
- ASP.NET Core API: Azure App Service (.NET 10)
- Database: Azure SQL Database

## Required GitHub configuration

Repository variables:

| Name | Example |
| --- | --- |
| `AZURE_WEBAPP_NAME` | `movieshop-api-example` |
| `AZURE_MVC_WEBAPP_NAME` | `movieshop-mvc-example` |
| `AZURE_API_BASE_URL` | `https://movieshop-api-example.azurewebsites.net` |

Repository secrets:

| Name | Purpose |
| --- | --- |
| `AZURE_CLIENT_ID` | OpenID Connect deployment identity |
| `AZURE_TENANT_ID` | Microsoft Entra tenant |
| `AZURE_SUBSCRIPTION_ID` | Azure subscription |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Static Web Apps deployment token |

Prefer GitHub-to-Azure OpenID Connect over a long-lived App Service publish profile.

## CI/CD model

The repository uses GitHub Actions as the equivalent of Azure Pipelines; moving the Git repository to Azure Repos is not required.

- `MovieShop CI` runs builds and tests for pull requests and non-main branches.
- Each main-branch deployment workflow builds and tests once, uploads a versioned pipeline artifact, then deploys that exact artifact.
- Angular's environment-specific API URL is injected into the downloaded artifact at deployment time, so QA and production do not require separate Angular builds.
- Deployment jobs target the GitHub `production` environment. Configure required reviewers on that environment to turn automatic production deployment into continuous delivery with a manual approval gate.
- Production deployment jobs use concurrency groups so two releases cannot modify the same App Service simultaneously.
- Smoke tests verify that the API and MVC URLs respond after deployment.

Recommended main-branch protection:

1. Require a pull request.
2. Require the `dotnet` and `angular` jobs from `MovieShop CI`.
3. Require at least one review.
4. Block merging when the branch is out of date.

The same compiled artifact should move through each environment. If QA or staging are added later, add deployment jobs that download the existing artifact rather than rebuilding the source.

## Required App Service configuration

Configure these settings in App Service; never commit their production values:

```text
ConnectionStrings__MovieShop=<Azure SQL connection string>
Jwt__SigningKey=<long random signing key>
Jwt__Issuer=MovieShop
Jwt__Audience=MovieShop.Client
Jwt__ExpirationMinutes=15
Cors__AllowedOrigins__0=https://<static-app>.azurestaticapps.net
ASPNETCORE_ENVIRONMENT=Production
```

Configure this setting in the MVC App Service:

```text
MovieShopApi__BaseUrl=https://<api-app>.azurewebsites.net/
ASPNETCORE_ENVIRONMENT=Production
```

The MVC site stores its normal login identity and the short-lived API access token inside an encrypted ASP.NET Core authentication cookie. The MVC server forwards that token to protected API routes. For a scaled-out or restart-resilient production deployment, persist ASP.NET Core Data Protection keys in shared Azure storage; otherwise replacing an instance can invalidate active MVC logins.

For stronger database security, configure an App Service managed identity and an Azure SQL connection using Microsoft Entra authentication instead of a SQL password.

## Database migrations

Apply migrations as a controlled deployment step before directing users to a new version. From a secured administrative environment with the Azure SQL connection configured:

```bash
cd MovieShopEFCore/MovieShop.Api
dotnet tool restore
dotnet ef database update
```

Do not place the Azure SQL password in a tracked file or automatically run migrations from every scaled-out application instance.

## Deployment order

1. Create a resource group and Azure SQL Database.
2. Create a .NET 10 App Service and configure its managed identity or connection string.
3. Apply EF Core migrations.
4. Set the App Service JWT configuration.
5. Set the GitHub API deployment identity and `AZURE_WEBAPP_NAME`.
6. Create a second .NET 10 App Service for MVC, configure `MovieShopApi__BaseUrl`, and set `AZURE_MVC_WEBAPP_NAME`.
7. Run the API workflow and verify `/api/movies`.
8. Run the MVC workflow and test registration, login, review, favorite, purchase, and profile history.
9. Create Static Web Apps and save its deployment token in GitHub if deploying Angular too.
10. Set `AZURE_API_BASE_URL` to the API App Service HTTPS URL.
11. Set the deployed Static Web Apps origin in API App Service CORS configuration.
12. Run the Angular workflow and test login, movies, details, pagination, and purchases.

Delete the resource group when the environment is no longer needed to stop charges.
