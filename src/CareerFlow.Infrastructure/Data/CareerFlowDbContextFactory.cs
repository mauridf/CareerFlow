using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace CareerFlow.Infrastructure.Data;

/// <summary>
/// Fábrica para criação do DbContext em tempo de design (migrations).
/// Lê a connection string do appsettings.json da API.
/// </summary>
public class CareerFlowDbContextFactory : IDesignTimeDbContextFactory<CareerFlowDbContext>
{
    public CareerFlowDbContext CreateDbContext(string[] args)
    {
        // Caminho para o projeto API onde está o appsettings.json
        var apiProjectPath = Path.Combine(
            Directory.GetCurrentDirectory(),
            "..", "CareerFlow.Api");

        // Configura o builder para ler appsettings
        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiProjectPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetSection("Database:ConnectionString").Value;

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'Database:ConnectionString' não encontrada. " +
                "Verifique o arquivo appsettings.json no projeto CareerFlow.Api.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<CareerFlowDbContext>();
        optionsBuilder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly(typeof(CareerFlowDbContext).Assembly.FullName);
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
        });

        return new CareerFlowDbContext(optionsBuilder.Options);
    }
}
