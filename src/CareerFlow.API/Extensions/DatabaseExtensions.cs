using CareerFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CareerFlow.API.Extensions;

public static class DatabaseExtensions
{
    public static async Task ApplyMigrationsAndSeedAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();

            // Aplicar migrations
            if ((await context.Database.GetPendingMigrationsAsync()).Any())
            {
                await context.Database.MigrateAsync();
                Console.WriteLine("Migrations aplicadas com sucesso.");
            }

            // Seed data
            await SeedData.SeedAsync(context);
            Console.WriteLine("Seed data aplicado com sucesso.");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Ocorreu um erro ao aplicar migrations ou seed data.");
            throw;
        }
    }
}