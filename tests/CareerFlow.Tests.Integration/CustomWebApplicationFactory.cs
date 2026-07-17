using CareerFlow.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace CareerFlow.Tests.Integration;

/// <summary>
/// Factory para criar a aplicação web para testes de integração.
/// Substitui o banco de dados real pelo Testcontainer.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly DatabaseFixture _databaseFixture;

    public CustomWebApplicationFactory(DatabaseFixture databaseFixture)
    {
        _databaseFixture = databaseFixture;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext existente
            services.RemoveAll<DbContextOptions<CareerFlowDbContext>>();
            services.RemoveAll<CareerFlowDbContext>();

            // Adiciona o DbContext com a connection string do Testcontainer
            services.AddDbContext<CareerFlowDbContext>(options =>
            {
                options.UseNpgsql(_databaseFixture.GetConnectionString());
            });

            // Garante que o banco está criado
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();
            db.Database.EnsureCreated();
        });
    }

    public Task InitializeAsync() => Task.CompletedTask;
    Task IAsyncLifetime.DisposeAsync() => Task.CompletedTask;
    public override ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
