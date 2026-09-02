using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
namespace EmployeeOperations.Infrastructure;

public sealed class EmployeeOperationsDbContextFactory : IDesignTimeDbContextFactory<EmployeeOperationsDbContext>
{
    public EmployeeOperationsDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__EmployeeOperations")
            ?? throw new InvalidOperationException(
                "Set ConnectionStrings__EmployeeOperations to a SQL Server connection string before running EF commands.");

        return new EmployeeOperationsDbContext(
            new DbContextOptionsBuilder<EmployeeOperationsDbContext>().UseSqlServer(connectionString).Options);
    }
}
