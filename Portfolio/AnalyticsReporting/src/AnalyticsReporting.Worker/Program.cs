using AnalyticsReporting.Core;
using AnalyticsReporting.Infrastructure;
using AnalyticsReporting.Worker;
using Microsoft.EntityFrameworkCore;
var b = Host.CreateApplicationBuilder(args); b.Services.AddDbContext<AnalyticsDbContext>(o => o.UseSqlServer(b.Configuration.GetConnectionString("Analytics") ?? throw new InvalidOperationException("Connection string required."))); b.Services.AddScoped<IJobStore, SqlJobStore>(); b.Services.AddScoped<CsvJobProcessor>(); b.Services.AddSingleton<IFileStore>(_ => new LocalFileStore(b.Configuration["Storage:Root"] ?? throw new InvalidOperationException("Storage root required."))); b.Services.AddHostedService<JobWorker>(); await b.Build().RunAsync();
