using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

[DisallowConcurrentExecution]
public class CleanupJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<CleanupJob> _logger;

    public CleanupJob(
        IServiceScopeFactory scopeFactory,
        ILogger<CleanupJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("🧹 [{JobName}] Iniciando limpeza de dados...", jobName);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();

            var cleanedItems = 0;

            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
            var processedMessages = await dbContext.OutboxMessages
                .Where(m => m.Status == "processed" && m.ProcessedAt < sevenDaysAgo)
                .ToListAsync(context.CancellationToken);

            dbContext.OutboxMessages.RemoveRange(processedMessages);
            cleanedItems += processedMessages.Count;
            _logger.LogInformation("🧹 [{JobName}] {Count} outbox messages removidas", jobName, processedMessages.Count);

            var ninetyDaysAgo = DateTime.UtcNow.AddDays(-90);
            var oldLogs = await dbContext.ActivityLogs
                .Where(l => l.CreatedAt < ninetyDaysAgo)
                .Take(1000)
                .ToListAsync(context.CancellationToken);

            dbContext.ActivityLogs.RemoveRange(oldLogs);
            cleanedItems += oldLogs.Count;
            _logger.LogInformation("🧹 [{JobName}] {Count} activity logs removidos", jobName, oldLogs.Count);

            var oneYearAgo = DateTime.UtcNow.AddDays(-365);
            var oldViews = await dbContext.ResumeViews
                .Where(v => v.CreatedAt < oneYearAgo)
                .Take(5000)
                .ToListAsync(context.CancellationToken);

            dbContext.ResumeViews.RemoveRange(oldViews);
            cleanedItems += oldViews.Count;
            _logger.LogInformation("🧹 [{JobName}] {Count} resume views antigas removidas", jobName, oldViews.Count);

            var oldDeletedUsers = await dbContext.Users
                .Where(u => u.DeletedAt.HasValue && u.DeletedAt < ninetyDaysAgo)
                .ToListAsync(context.CancellationToken);

            if (oldDeletedUsers.Count > 0)
            {
                dbContext.Users.RemoveRange(oldDeletedUsers);
                cleanedItems += oldDeletedUsers.Count;
                _logger.LogInformation("🧹 [{JobName}] {Count} usuários deletados permanentemente (LGPD)",
                    jobName, oldDeletedUsers.Count);
            }

            await dbContext.SaveChangesAsync(context.CancellationToken);

            var tempDir = Path.Combine(Path.GetTempPath(), "CareerFlow");
            if (Directory.Exists(tempDir))
            {
                var tempCutoff = DateTime.UtcNow.AddDays(-7);
                var deletedFiles = 0;

                foreach (var file in Directory.GetFiles(tempDir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (File.GetLastWriteTimeUtc(file) < tempCutoff)
                        {
                            File.Delete(file);
                            deletedFiles++;
                        }
                    }
                    catch { }
                }

                foreach (var dir in Directory.GetDirectories(tempDir, "*", SearchOption.AllDirectories).Reverse())
                {
                    if (!Directory.EnumerateFileSystemEntries(dir).Any())
                    {
                        Directory.Delete(dir);
                    }
                }

                _logger.LogInformation("🧹 [{JobName}] {Count} arquivos temporários removidos", jobName, deletedFiles);
                cleanedItems += deletedFiles;
            }

            _logger.LogInformation("✅ [{JobName}] Limpeza concluída: {Total} itens removidos", jobName, cleanedItems);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro durante limpeza", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
