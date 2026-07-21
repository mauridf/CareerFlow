using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

[DisallowConcurrentExecution]
public class PremiumExpirationJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PremiumExpirationJob> _logger;

    public PremiumExpirationJob(
        IServiceScopeFactory scopeFactory,
        ILogger<PremiumExpirationJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("⭐ [{JobName}] Verificando expiração de planos premium...", jobName);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();

            var now = DateTime.UtcNow;

            var expiredUsers = await dbContext.Users
                .Where(u => u.IsPremium
                    && u.PremiumUntil.HasValue
                    && u.PremiumUntil.Value <= now)
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("⭐ [{JobName}] {Count} planos premium expirados encontrados", jobName, expiredUsers.Count);

            foreach (var user in expiredUsers)
            {
                user.DeactivatePremium();

                dbContext.ActivityLogs.Add(new CareerFlow.Core.Entities.ActivityLog
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Action = "premium_expired",
                    EntityType = "User",
                    EntityId = user.Id,
                    Details = $"{{\"premiumUntil\": \"{user.PremiumUntil:O}\"}}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                _logger.LogInformation("⭐ [{JobName}] Premium desativado para: {Email}", jobName, user.Email);
            }

            if (expiredUsers.Count > 0)
            {
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }

            var sevenDaysFromNow = now.AddDays(7);
            var expiringSoon = await dbContext.Users
                .Where(u => u.IsPremium
                    && u.PremiumUntil.HasValue
                    && u.PremiumUntil.Value > now
                    && u.PremiumUntil.Value <= sevenDaysFromNow)
                .ToListAsync(context.CancellationToken);

            foreach (var user in expiringSoon)
            {
                _logger.LogInformation(
                    "⭐ [{JobName}] Premium expira em breve: {Email} - {Days} dias restantes",
                    jobName, user.Email, (user.PremiumUntil!.Value - now).Days);
            }

            _logger.LogInformation("✅ [{JobName}] Verificação concluída. {Expired} expirados, {Soon} expiram em breve",
                jobName, expiredUsers.Count, expiringSoon.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao verificar expirações", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
