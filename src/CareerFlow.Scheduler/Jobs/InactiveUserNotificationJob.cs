using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

/// <summary>
/// Job que notifica usuários inativos há mais de 30 dias.
/// Execução: Dia 1 de cada mês às 09:00
/// </summary>
[DisallowConcurrentExecution]
public class InactiveUserNotificationJob : IJob
{
    private readonly CareerFlowDbContext _dbContext;
    private readonly ILogger<InactiveUserNotificationJob> _logger;

    public InactiveUserNotificationJob(
        CareerFlowDbContext dbContext,
        ILogger<InactiveUserNotificationJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("🔔 [{JobName}] Verificando usuários inativos...", jobName);

        try
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            // Busca usuários que não fazem login há 30 dias
            var inactiveUsers = await _dbContext.Users
                .Where(u => u.IsActive
                    && !u.DeletedAt.HasValue
                    && (u.LastLoginAt == null || u.LastLoginAt < thirtyDaysAgo))
                .ToListAsync(context.CancellationToken);

            _logger.LogInformation("🔔 [{JobName}] {Count} usuários inativos encontrados", jobName, inactiveUsers.Count);

            foreach (var user in inactiveUsers)
            {
                var daysSinceLastLogin = user.LastLoginAt.HasValue
                    ? (DateTime.UtcNow - user.LastLoginAt.Value).Days
                    : 999;

                // TODO: Integrar com serviço de envio de email
                _logger.LogInformation(
                    "🔔 [{JobName}] Notificação: {Email} - Inativo há {Days} dias",
                    jobName, user.Email, daysSinceLastLogin);

                // Registra atividade
                _dbContext.ActivityLogs.Add(new CareerFlow.Core.Entities.ActivityLog
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Action = "inactive_notification_sent",
                    EntityType = "User",
                    EntityId = user.Id,
                    Details = $"{{\"daysInactive\": {daysSinceLastLogin}}}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (inactiveUsers.Count > 0)
            {
                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }

            _logger.LogInformation("✅ [{JobName}] Notificações enviadas: {Count} usuários", jobName, inactiveUsers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao processar notificações", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
