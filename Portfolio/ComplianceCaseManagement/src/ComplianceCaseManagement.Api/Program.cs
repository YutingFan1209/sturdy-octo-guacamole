using System.Text;
using ComplianceCaseManagement.Api;
using ComplianceCaseManagement.Cases;
using ComplianceCaseManagement.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
var b = WebApplication.CreateBuilder(args); b.Services.AddControllers(); b.Services.AddProblemDetails(); b.Services.AddExceptionHandler<ApiExceptionHandler>(); b.Services.AddDbContext<CaseDbContext>(o => o.UseSqlServer(b.Configuration.GetConnectionString("ComplianceCases") ?? throw new InvalidOperationException("Connection string required."))); b.Services.AddScoped<ICaseStore, SqlCaseStore>(); b.Services.AddScoped<CaseService>(); b.Services.AddSingleton(TimeProvider.System); var key = b.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("JWT key required."); b.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => o.TokenValidationParameters = new() { ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), ValidateIssuer = false, ValidateAudience = false, RoleClaimType = "role", NameClaimType = "sub" }); b.Services.AddAuthorizationBuilder().AddPolicy("Analyst", p => p.RequireRole("ComplianceAnalyst")); var app = b.Build(); app.UseExceptionHandler(); app.UseAuthentication(); app.UseAuthorization(); app.MapControllers(); app.Run(); public partial class Program;
