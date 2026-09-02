using Microsoft.EntityFrameworkCore;
using ReconciliationPlatform.Core;
using ReconciliationPlatform.Infrastructure;
using ReconciliationPlatform.Reconciler;
var b = Host.CreateApplicationBuilder(args); b.Services.AddDbContext<ReconciliationDbContext>(o => o.UseSqlServer(b.Configuration.GetConnectionString("Reconciliation") ?? throw new InvalidOperationException("Connection string required."))); b.Services.AddScoped<IReconciliationStore, SqlReconciliationStore>(); b.Services.AddScoped<ReconciliationService>(); b.Services.AddSingleton(TimeProvider.System); b.Services.AddHostedService<ReconciliationWorker>(); await b.Build().RunAsync();
