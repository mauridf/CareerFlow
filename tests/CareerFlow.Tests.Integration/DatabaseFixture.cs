using CareerFlow.Infrastructure.Data;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace CareerFlow.Tests.Integration;

/// <summary>
/// Fixture compartilhada que gerencia o container PostgreSQL para testes.
/// Implementa IAsyncLifetime para inicialização e limpeza assíncronas.
/// </summary>
public class DatabaseFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgresContainer;
    private string _connectionString = string.Empty;

    public CareerFlowDbContext DbContext { get; private set; } = null!;

    public DatabaseFixture()
    {
        // Configura o container PostgreSQL
        _postgresContainer = new PostgreSqlBuilder()
            .WithImage("postgres:16-alpine")
            .WithDatabase("careerflow_test")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .WithPortBinding(0, 5432) // Porta aleatória
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithCleanUp(true)
            .Build();
    }

    public string GetConnectionString() => _connectionString;

    public async Task InitializeAsync()
    {
        // Inicia o container
        await _postgresContainer.StartAsync();

        // Obtém a connection string do container
        _connectionString = _postgresContainer.GetConnectionString();

        // Configura o DbContext
        var options = new DbContextOptionsBuilder<CareerFlowDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        DbContext = new CareerFlowDbContext(options, null, null);

        // Cria as tabelas
        await DbContext.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        if (DbContext != null)
            await DbContext.DisposeAsync();

        if (_postgresContainer != null)
            await _postgresContainer.DisposeAsync();
    }
}

/// <summary>
/// Collection fixture para compartilhar o container entre testes.
/// </summary>
[CollectionDefinition("Database")]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}
