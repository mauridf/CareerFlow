using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

[DisallowConcurrentExecution]
public class ResumeSuggestionJob : IJob
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ResumeSuggestionJob> _logger;

    public ResumeSuggestionJob(
        IServiceScopeFactory scopeFactory,
        ILogger<ResumeSuggestionJob> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("💡 [{JobName}] Gerando sugestões de melhoria...", jobName);

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CareerFlowDbContext>();
            var analyzer = scope.ServiceProvider.GetRequiredService<IResumeAnalyzerService>();

            var publishedPersons = await dbContext.ResumeAnalytics
                .Where(a => a.Status == CareerFlow.Core.Enums.ResumeStatus.Published)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.User)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.Experiences)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.Educations)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.Skills)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.Certificates)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.Languages)
                .Include(a => a.Person)
                    .ThenInclude(p => p!.SocialNetworks)
                .Where(a => a.Person != null
                    && a.Person.User != null
                    && a.Person.User.IsActive
                    && !a.Person.User.DeletedAt.HasValue)
                .Select(a => a.Person!)
                .ToListAsync(context.CancellationToken);

            var totalSuggestions = 0;

            foreach (var person in publishedPersons)
            {
                var results = await analyzer.GenerateSuggestionsAsync(person.Id, context.CancellationToken);

                if (results.Count > 0)
                {
                    var oldSuggestions = await dbContext.ResumeSuggestions
                        .Where(s => s.PersonId == person.Id && !s.IsApplied)
                        .ToListAsync(context.CancellationToken);

                    dbContext.ResumeSuggestions.RemoveRange(oldSuggestions);

                    foreach (var result in results)
                    {
                        dbContext.ResumeSuggestions.Add(ResumeSuggestion.Create(
                            person.Id,
                            result.Category,
                            result.Title,
                            result.Description,
                            result.Priority));
                    }

                    totalSuggestions += results.Count;
                }
            }

            await dbContext.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ [{JobName}] {Count} sugestões geradas para {Persons} currículos publicados",
                jobName, totalSuggestions, publishedPersons.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao gerar sugestões", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }
}
