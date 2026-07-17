using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

/// <summary>
/// Job que atualiza analytics de currículos diariamente.
/// Calcula total views, unique views, PDF downloads e score ATS.
/// Execução: Diária às 02:00
/// </summary>
[DisallowConcurrentExecution]
public class ResumeAnalyticsJob : IJob
{
    private readonly CareerFlowDbContext _dbContext;
    private readonly ILogger<ResumeAnalyticsJob> _logger;

    public ResumeAnalyticsJob(
        CareerFlowDbContext dbContext,
        ILogger<ResumeAnalyticsJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("📊 [{JobName}] Iniciando atualização de analytics...", jobName);

        try
        {
            // Busca todas as pessoas com analytics
            var analyticsList = await _dbContext.ResumeAnalytics
                .Include(a => a.Person)
                .ToListAsync(context.CancellationToken);

            var updatedCount = 0;

            foreach (var analytics in analyticsList)
            {
                var personId = analytics.PersonId;

                // Atualiza contagem de views
                var totalViews = await _dbContext.ResumeViews
                    .CountAsync(v => v.PersonId == personId, context.CancellationToken);

                var uniqueViews = await _dbContext.ResumeViews
                    .Where(v => v.PersonId == personId)
                    .Select(v => v.IpAddress)
                    .Distinct()
                    .CountAsync(context.CancellationToken);

                var pdfDownloads = await _dbContext.ResumeViews
                    .CountAsync(v => v.PersonId == personId && v.PdfDownloaded, context.CancellationToken);

                var averageDuration = await _dbContext.ResumeViews
                    .Where(v => v.PersonId == personId && v.ViewDurationSeconds.HasValue)
                    .Select(v => v.ViewDurationSeconds!.Value)
                    .DefaultIfEmpty(0)
                    .AverageAsync(context.CancellationToken);

                analytics.UpdateMetrics(totalViews, uniqueViews, pdfDownloads, (int)Math.Round(averageDuration));

                // Atualiza score ATS
                var person = analytics.Person;
                if (person != null)
                {
                    var score = CalculateAtsScore(person);
                    analytics.UpdateAtsAnalysis(score, score, null, null);
                }
                updatedCount++;
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ [{JobName}] Analytics atualizados para {Count} currículos", jobName, updatedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao atualizar analytics", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }

    private static int CalculateAtsScore(Person person)
    {
        int score = 0;

        // Informações de contato (20 pontos)
        if (!string.IsNullOrWhiteSpace(person.City)) score += 10;
        if (!string.IsNullOrWhiteSpace(person.Phone)) score += 5;
        if (person.User?.Email != null) score += 5;

        // Resumo profissional (15 pontos)
        if (!string.IsNullOrWhiteSpace(person.ProfessionalSummary))
        {
            var summaryLength = person.ProfessionalSummary.Length;
            if (summaryLength >= 100 && summaryLength <= 2000) score += 15;
            else if (summaryLength >= 100) score += 10;
            else score += 5;
        }

        // Experiências (30 pontos)
        var expCount = person.Experiences?.Count ?? 0;
        if (expCount >= 3) score += 30;
        else if (expCount >= 2) score += 20;
        else if (expCount >= 1) score += 10;

        // Formação (15 pontos)
        var eduCount = person.Educations?.Count ?? 0;
        if (eduCount >= 2) score += 15;
        else if (eduCount >= 1) score += 10;

        // Habilidades (15 pontos)
        var skillCount = person.Skills?.Count ?? 0;
        if (skillCount >= 5) score += 15;
        else if (skillCount >= 3) score += 10;
        else if (skillCount >= 1) score += 5;

        // Certificações (5 pontos bônus)
        if ((person.Certificates?.Count ?? 0) >= 1) score += 5;

        return Math.Min(score, 100);
    }
}
