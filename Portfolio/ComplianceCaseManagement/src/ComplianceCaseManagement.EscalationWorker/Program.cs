using ComplianceCaseManagement.Cases;
using ComplianceCaseManagement.EscalationWorker;
using ComplianceCaseManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;
var b = Host.CreateApplicationBuilder(args); b.Services.AddDbContext<CaseDbContext>(o => o.UseSqlServer(b.Configuration.GetConnectionString("ComplianceCases") ?? throw new InvalidOperationException("Connection string required."))); b.Services.AddScoped<ICaseStore, SqlCaseStore>(); b.Services.AddScoped<CaseService>(); b.Services.AddSingleton(TimeProvider.System); b.Services.AddHostedService<EscalationWorker>(); await b.Build().RunAsync();
