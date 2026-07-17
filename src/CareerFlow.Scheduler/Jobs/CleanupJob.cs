using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

/// <summary>
/// Job que realiza limpeza de dados antigos.
/// Remove logs antigos, outbox messages processadas e dados temporários.
/// Execução: Todo domingo às 03:00
/// </summary>
[DisallowConcurrentExecution]
public class CleanupJob : IJob
{
    private readonly CareerFlowDbContext _dbContext;
    private readonly ILogger<CleanupJob> _logger;

    public CleanupJob(
        CareerFlowDbContext dbContext,
        ILogger<CleanupJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("🧹 [{JobName}] Iniciando limpeza de dados...", jobName);

        try
        {
            var cleanedItems = 0;

            // 1. Remove outbox messages processadas com mais de 7 dias
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var processedMessages = await _dbContext.OutboxMessages
                .Where(m => m.Status == "processed" && m.ProcessedAt < sevenDaysAgo)
                .ToListAsync(context.CancellationToken);

            _dbContext.OutboxMessages.RemoveRange(processedMessages);
            cleanedItems += processedMessages.Count;
            _logger.LogInformation("🧹 [{JobName}] {Count} outbox messages removidas", jobName, processedMessages.Count);

            // 2. Remove activity logs com mais de 90 dias
            var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
            var oldLogs = await _dbContext.ActivityLogs
                .Where(l => l.CreatedAt < ninetyDaysAgo)
                .Take(1000) // Limita para não travar o banco
                .ToListAsync(context.CancellationToken);

            _dbContext.ActivityLogs.RemoveRange(oldLogs);
            cleanedItems += oldLogs.Count;
            _logger.LogInformation("🧹 [{JobName}] {Count} activity logs removidos", jobName, oldLogs.Count);

            // 3. Remove resume views com mais de 180 dias
            var sixMonthsAgo = DateTime.UtcNow.AddDays(-180);
            var oldViews = await _dbContext.ResumeViews
                .Where(v => v.CreatedAt < sixMonthsAgo)
                .Take(5000)
                .ToListAsync(context.CancellationToken);

            _dbContext.ResumeViews.RemoveRange(oldViews);
            cleanedItems += oldViews.Count;
            _logger.LogInformation("🧹 [{JobName}] {Count} resume views removidas", jobName, oldViews.Count);

            // 4. Remove usuários com soft delete há mais de 90 dias (LGPD)
            var oldDeletedUsers = await _dbContext.Users
                .Where(u => u.DeletedAt.HasValue && u.DeletedAt < ninetyDaysAgo)
                .ToListAsync(context.CancellationToken);

            if (oldDeletedUsers.Count > 0)
            {
                _dbContext.Users.RemoveRange(oldDeletedUsers);
                cleanedItems += oldDeletedUsers.Count;
                _logger.LogInformation("🧹 [{JobName}] {Count} usuários deletados permanentemente (LGPD)",
                    jobName, oldDeletedUsers.Count);
            }

            // Salva todas as alterações
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ [{JobName}] Limpeza concluída: {Total} itens removidos", jobName, cleanedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro durante limpeza", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
