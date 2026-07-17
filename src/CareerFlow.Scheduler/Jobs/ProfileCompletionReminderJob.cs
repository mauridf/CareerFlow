using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

/// <summary>
/// Job que envia lembretes para usuários com perfil incompleto.
/// Execução: Toda segunda-feira às 08:00
/// </summary>
[DisallowConcurrentExecution]
public class ProfileCompletionReminderJob : IJob
{
    private readonly CareerFlowDbContext _dbContext;
    private readonly ILogger<ProfileCompletionReminderJob> _logger;

    public ProfileCompletionReminderJob(
        CareerFlowDbContext dbContext,
        ILogger<ProfileCompletionReminderJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("📧 [{JobName}] Verificando perfis incompletos...", jobName);

        try
        {
            // Busca pessoas com perfil abaixo de 80%
            var persons = await _dbContext.Persons
                .Include(p => p.User)
                .Where(p => p.User != null && p.User.IsActive && !p.User.DeletedAt.HasValue)
                .ToListAsync(context.CancellationToken);

            var incompleteProfiles = persons
                .Where(p => p.CalculateCompletionPercentage() < 80)
                .ToList();

            _logger.LogInformation("📧 [{JobName}] {Count} perfis incompletos encontrados", jobName, incompleteProfiles.Count);

            foreach (var person in incompleteProfiles)
            {
                var completion = person.CalculateCompletionPercentage();
                var user = person.User!;

                // TODO: Integrar com serviço de envio de email
                _logger.LogInformation(
                    "📧 [{JobName}] Lembrete: Usuário {Email} - Perfil {Percentage}% completo",
                    jobName, user.Email, completion);

                // Registra atividade
                _dbContext.ActivityLogs.Add(new CareerFlow.Core.Entities.ActivityLog
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
                await _dbContext.SaveChangesAsync(context.CancellationToken);
            }

            _logger.LogInformation("✅ [{JobName}] Lembretes processados: {Count} usuários", jobName, incompleteProfiles.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao processar lembretes", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
