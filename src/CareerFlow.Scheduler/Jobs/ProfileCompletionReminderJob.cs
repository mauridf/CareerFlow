using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

[DisallowConcurrentExecution]
public class ProfileCompletionReminderJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ProfileCompletionReminderJob> _logger;

    public ProfileCompletionReminderJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ProfileCompletionReminderJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("📧 [{JobName}] Verificando perfis incompletos...", jobName);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();

            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            var persons = await dbContext.Persons
                .Include(p => p.User)
                .Where(p => p.User != null && p.User.IsActive && !p.User.DeletedAt.HasValue)
                .ToListAsync(context.CancellationToken);

            var incompleteProfiles = persons
                .Where(p => p.CreatedAt < sevenDaysAgo && p.CalculateCompletionPercentage() < 60)
                .ToList();

            _logger.LogInformation("📧 [{JobName}] {Count} perfis incompletos encontrados", jobName, incompleteProfiles.Count);

            foreach (var person in incompleteProfiles)
            {
                var completion = person.CalculateCompletionPercentage();
                var user = person.User!;

                _logger.LogInformation(
                    "📧 [{JobName}] Sugestão para {Email} - Perfil {Percentage}% completo (criado há {Days} dias)",
                    jobName, user.Email, completion, (DateTime.UtcNow - person.CreatedAt).Days);

                dbContext.ActivityLogs.Add(new CareerFlow.Core.Entities.ActivityLog
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Action = "profile_reminder_sent",
                    EntityType = "Person",
                    EntityId = person.Id,
                    Details = $"{{\"completionPercentage\": {completion}}}",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            if (incompleteProfiles.Count > 0)
            {
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }

            _logger.LogInformation("✅ [{JobName}] Lembretes processados: {Count} usuários com perfil < 60%", jobName, incompleteProfiles.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao processar lembretes", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
