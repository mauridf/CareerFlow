using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Quartz;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Scheduler.Jobs;

/// <summary>
/// Job que gera sugestões de melhoria automáticas para currículos.
/// Execução: Diária às 05:00
/// </summary>
[DisallowConcurrentExecution]
public class ResumeSuggestionJob : IJob
{
    private readonly CareerFlowDbContext _dbContext;
    private readonly ILogger<ResumeSuggestionJob> _logger;

    public ResumeSuggestionJob(
        CareerFlowDbContext dbContext,
        ILogger<ResumeSuggestionJob> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        _logger.LogInformation("💡 [{JobName}] Gerando sugestões de melhoria...", jobName);

        try
        {
            // Busca todos os perfis ativos
            var persons = await _dbContext.Persons
                .Include(p => p.User)
                .Include(p => p.Experiences)
                .Include(p => p.Educations)
                .Include(p => p.Skills)
                .Include(p => p.Certificates)
                .Include(p => p.Languages)
                .Where(p => p.User != null && p.User.IsActive && !p.User.DeletedAt.HasValue)
                .ToListAsync(context.CancellationToken);

            var totalSuggestions = 0;

            foreach (var person in persons)
            {
                var suggestions = GenerateSuggestions(person);

                if (suggestions.Count > 0)
                {
                    // Remove sugestões antigas não aplicadas
                    var oldSuggestions = await _dbContext.ResumeSuggestions
                        .Where(s => s.PersonId == person.Id && !s.IsApplied)
                        .ToListAsync(context.CancellationToken);

                    _dbContext.ResumeSuggestions.RemoveRange(oldSuggestions);

                    // Adiciona novas sugestões
                    foreach (var suggestion in suggestions)
                    {
                        _dbContext.ResumeSuggestions.Add(suggestion);
                    }

                    totalSuggestions += suggestions.Count;
                }
            }

            await _dbContext.SaveChangesAsync(context.CancellationToken);

            _logger.LogInformation("✅ [{JobName}] {Count} sugestões geradas para {Persons} perfis",
                jobName, totalSuggestions, persons.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ [{JobName}] Erro ao gerar sugestões", jobName);
            throw new JobExecutionException(ex) { RefireImmediately = false };
        }
    }

    private static List<ResumeSuggestion> GenerateSuggestions(Person person)
    {
        var suggestions = new List<ResumeSuggestion>();

        // Verifica resumo profissional
        if (string.IsNullOrWhiteSpace(person.ProfessionalSummary))
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "summary",
                "Adicione um resumo profissional",
                "Um bom resumo profissional ajuda recrutadores a entenderem rapidamente seu perfil. " +
                "Inclua suas principais habilidades, anos de experiência e objetivos de carreira.",
                "high"));
        }
        else if (person.ProfessionalSummary.Length < 200)
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "summary",
                "Resumo profissional muito curto",
                "Seu resumo tem apenas " + person.ProfessionalSummary.Length + " caracteres. " +
                "Recomendamos pelo menos 200 caracteres para descrever bem seu perfil.",
                "medium"));
        }

        // Verifica experiências
        var expCount = person.Experiences?.Count ?? 0;
        if (expCount == 0)
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "experience",
                "Adicione experiências profissionais",
                "Experiências profissionais são a seção mais importante do currículo. " +
                "Adicione pelo menos uma experiência.",
                "high"));
        }
        else if (expCount < 2)
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "experience",
                "Adicione mais experiências",
                "Considere adicionar mais experiências profissionais para enriquecer seu currículo.",
                "medium"));
        }

        // Verifica habilidades
        var skillCount = person.Skills?.Count ?? 0;
        if (skillCount < 5)
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "skills",
                "Adicione mais habilidades",
                $"Você tem {skillCount} habilidades. Recomendamos pelo menos 5 para um bom score ATS.",
                "medium"));
        }

        // Verifica soft skills
        if (person.Skills != null && person.Skills.Any() &&
            person.Skills.All(s => s.Category != SkillCategory.SoftSkills))
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "skills",
                "Adicione soft skills",
                "Soft skills como comunicação, liderança e trabalho em equipe são valorizadas por recrutadores.",
                "low"));
        }

        // Verifica certificações
        if ((person.Certificates?.Count ?? 0) == 0)
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "certificates",
                "Adicione certificações",
                "Certificações relevantes podem aumentar seu score ATS e destacar seu perfil.",
                "low"));
        }

        // Verifica foto de perfil
        if (string.IsNullOrWhiteSpace(person.PhotoUrl))
        {
            suggestions.Add(ResumeSuggestion.Create(person.Id, "profile",
                "Adicione uma foto de perfil",
                "Uma foto profissional aumenta a credibilidade do seu currículo.",
                "low"));
        }

        return suggestions;
    }
}
