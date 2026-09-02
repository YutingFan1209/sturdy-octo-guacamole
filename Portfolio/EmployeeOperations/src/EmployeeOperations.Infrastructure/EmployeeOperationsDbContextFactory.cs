using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace EmployeeOperations.Infrastructure;

public sealed class EmployeeOperationsDbContextFactory : IDesignTimeDbContextFactory<EmployeeOperationsDbContext>
{
    public EmployeeOperationsDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<EmployeeOperationsDbContext>()
        .UseSqlServer("Server=localhost;Database=EmployeeOperations;Trusted_Connection=True;TrustServerCertificate=True").Options);
}
