using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Core.Enums;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

/// <summary>
/// Job que verifica e processa expiração de planos premium.
/// Execução: Diária às 04:00
/// </summary>
[DisallowConcurrentExecution]
public class PremiumExpirationJob : IJob
{
    private readonly CareerFlowDbContext _dbContext;
    private readonly ILogger<PremiumExpirationJob> _logger;

    public PremiumExpirationJob(
        CareerFlowDbContext dbContext,
        ILogger<PremiumExpirationJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("⭐ [{JobName}] Verificando expiração de planos premium...", jobName);

        try
        {
            var now = DateTime.UtcNow;

            // Busca usuários premium expirados
            var expiredUsers = await _dbContext.Users
                .Where(u => u.IsPremium
                    && u.PremiumUntil.HasValue
                    && u.PremiumUntil.Value <= now)
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("⭐ [{JobName}] {Count} planos premium expirados", jobName, expiredUsers.Count);

            foreach (var user in expiredUsers)
            {
                user.DeactivatePremium();

                // Registra atividade
                _dbContext.ActivityLogs.Add(new CareerFlow.Core.Entities.ActivityLog
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

                _logger.LogInformation("⭐ [{JobName}] Premium expirado para: {Email}", jobName, user.Email);
            }

            if (expiredUsers.Count > 0)
            {
                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }

            // Avisa usuários que expiram em 7 dias
            var sevenDaysFromNow = now.AddDays(7);
            var expiringSoon = await _dbContext.Users
                .Where(u => u.IsPremium
                    && u.PremiumUntil.HasValue
                    && u.PremiumUntil.Value > now
                    && u.PremiumUntil.Value <= sevenDaysFromNow)
                .ToListAsync(context.CancellationToken);

            foreach (var user in expiringSoon)
            {
                // TODO: Enviar email de aviso
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
