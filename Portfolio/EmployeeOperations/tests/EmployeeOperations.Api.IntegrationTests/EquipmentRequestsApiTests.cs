using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using EmployeeOperations.Application;
using EmployeeOperations.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EmployeeOperations.Api.IntegrationTests;

[TestClass]
public sealed class EquipmentRequestsApiTests
{
    private WebApplicationFactory<Program> _factory = null!;
    [TestInitialize] public void Setup() => _factory = new ApiFactory();
    [TestCleanup] public void Cleanup() => _factory.Dispose();
    [TestMethod]
    public async Task Workflow_authorization_history_and_stale_version_are_enforced()
    {
        var employee = Client("Employee", "employee-1");
        var response = await employee.PostAsJsonAsync("/api/equipment-requests", new { item = "Laptop", justification = "Build work" });
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<EquipmentRequestDto>(); Assert.IsNotNull(created);
        var submitted = await (await employee.PostAsJsonAsync($"/api/equipment-requests/{created.Id}/submit", new { expectedVersion = 0L })).Content.ReadFromJsonAsync<EquipmentRequestDto>(); Assert.IsNotNull(submitted);
        Assert.AreEqual(HttpStatusCode.Forbidden, (await employee.PostAsJsonAsync($"/api/equipment-requests/{created.Id}/approve", new { expectedVersion = 1L })).StatusCode);
        var manager = Client("Manager", "manager-1");
        Assert.AreEqual(HttpStatusCode.OK, (await manager.PostAsJsonAsync($"/api/equipment-requests/{created.Id}/approve", new { expectedVersion = 1L })).StatusCode);
        Assert.AreEqual(HttpStatusCode.Conflict, (await manager.PostAsJsonAsync($"/api/equipment-requests/{created.Id}/reject", new { expectedVersion = 1L })).StatusCode);
        var complete = await Client("Operations", "ops-1").PostAsJsonAsync($"/api/equipment-requests/{created.Id}/complete", new { expectedVersion = 2L, reason = "Asset A-10 issued" });
        var result = await complete.Content.ReadFromJsonAsync<EquipmentRequestDto>();
        Assert.AreEqual(HttpStatusCode.OK, complete.StatusCode); Assert.IsNotNull(result); Assert.AreEqual(EquipmentRequestStatus.Completed, result.Status); Assert.HasCount(3, result.History);
    }
    [TestMethod]
    public async Task Anonymous_is_unauthorized_and_missing_id_is_not_found()
    {
        Assert.AreEqual(HttpStatusCode.Unauthorized, (await _factory.CreateClient().PostAsJsonAsync("/api/equipment-requests", new { item = "Laptop", justification = "Needed" })).StatusCode);
        Assert.AreEqual(HttpStatusCode.NotFound, (await Client("Employee", "employee-1").GetAsync($"/api/equipment-requests/{Guid.NewGuid()}")).StatusCode);
    }
    private HttpClient Client(string role, string actor) { var c = _factory.CreateClient(); c.DefaultRequestHeaders.Add("X-Test-Role", role); c.DefaultRequestHeaders.Add("X-Test-Actor", actor); return c; }
}

file sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing").UseSetting("Jwt:SigningKey", new string('x', 32)).UseSetting("ConnectionStrings:EmployeeOperations", "Server=unused");
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication(o => { o.DefaultAuthenticateScheme = "Test"; o.DefaultChallengeScheme = "Test"; }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });
            services.AddSingleton<IEquipmentRequestRepository, MemoryRepository>();
        });
    }
}
file sealed class TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> o, ILoggerFactory l, UrlEncoder e) : AuthenticationHandler<AuthenticationSchemeOptions>(o, l, e)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Test-Role", out var role)) return Task.FromResult(AuthenticateResult.NoResult());
        var identity = new ClaimsIdentity([new(ClaimTypes.NameIdentifier, Request.Headers["X-Test-Actor"].ToString()), new(ClaimTypes.Role, role.ToString())], Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)));
    }
}
file sealed class MemoryRepository : IEquipmentRequestRepository
{
    private readonly Dictionary<Guid, EquipmentRequest> _items = [];
    public Task AddAsync(EquipmentRequest r, CancellationToken ct) { _items.Add(r.Id, r); return Task.CompletedTask; }
    public Task<EquipmentRequest?> FindAsync(Guid id, CancellationToken ct) => Task.FromResult(_items.GetValueOrDefault(id));
    public Task SaveChangesAsync(long expectedVersion, CancellationToken ct) => Task.CompletedTask;
}
