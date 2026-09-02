using System.Text;
using EmployeeOperations.Api;
using EmployeeOperations.Application;
using EmployeeOperations.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddHealthChecks().AddDbContextCheck<EmployeeOperationsDbContext>();
builder.Services.AddDbContext<EmployeeOperationsDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("EmployeeOperations") ?? throw new InvalidOperationException("ConnectionStrings:EmployeeOperations is required.")));
builder.Services.AddScoped<IEquipmentRequestRepository, EquipmentRequestRepository>();
builder.Services.AddScoped<EquipmentRequestService>();
builder.Services.AddSingleton(TimeProvider.System);
var signingKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey must be supplied via user secrets or environment variables.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(1),
        RoleClaimType = "role",
        NameClaimType = "sub"
    });
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Employee", p => p.RequireRole("Employee"))
    .AddPolicy("Manager", p => p.RequireRole("Manager"))
    .AddPolicy("Operations", p => p.RequireRole("Operations"));
builder.Services.AddCors(options => options.AddPolicy("AngularClient", policy => policy
    .WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
    .AllowAnyHeader().AllowAnyMethod()));
var app = builder.Build();
app.UseExceptionHandler(); app.UseHttpsRedirection(); app.UseCors("AngularClient");
app.UseAuthentication(); app.UseAuthorization();
app.MapControllers(); app.MapHealthChecks("/health"); app.Run();
public partial class Program;
