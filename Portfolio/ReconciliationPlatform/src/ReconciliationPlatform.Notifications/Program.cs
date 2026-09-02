using Microsoft.EntityFrameworkCore;
using ReconciliationPlatform.Core;
using ReconciliationPlatform.Infrastructure;
using ReconciliationPlatform.Notifications;
var b = Host.CreateApplicationBuilder(args); b.Services.AddDbContext<ReconciliationDbContext>(o => o.UseSqlServer(b.Configuration.GetConnectionString("Reconciliation") ?? throw new InvalidOperationException("Connection string required."))); b.Services.AddScoped<IReconciliationStore, SqlReconciliationStore>(); b.Services.AddScoped<NotificationService>(); b.Services.AddSingleton(TimeProvider.System); b.Services.AddSingleton<INotificationGateway>(_ => new HttpNotificationGateway(new HttpClient { BaseAddress = new Uri(b.Configuration["Notification:BaseUrl"] ?? throw new InvalidOperationException("Notification URL required.")) })); b.Services.AddHostedService<NotificationWorker>(); await b.Build().RunAsync();
