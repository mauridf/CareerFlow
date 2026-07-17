using Microsoft.Extensions.Logging;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;

namespace CareerFlow.Infrastructure.External.Analytics;

public class ResumeAnalyzerService : IResumeAnalyzerService
{
    private readonly IPersonRepository _personRepo;
    private readonly ILogger<ResumeAnalyzerService> _logger;

    private static readonly string[] TechKeywords =
    [
        "c#", ".net", "python", "java", "javascript", "typescript", "react", "angular",
        "sql", "azure", "aws", "docker", "kubernetes", "git", "rest", "api", "microservices",
        "entity framework", "redis", "rabbitmq", "ci/cd", "testing", "agile", "scrum"
    ];

    private static readonly string[] SoftSkills =
    [
        "comunicação", "liderança", "trabalho em equipe", "resolução de problemas",
        "comunicação", "leadership", "teamwork", "problem solving", "proatividade",
        "organização", "comunicação", "colaboração", "adaptabilidade"
    ];

    public ResumeAnalyzerService(IPersonRepository personRepo, ILogger<ResumeAnalyzerService> logger)
    {
        _personRepo = personRepo;
        _logger = logger;
    }

    public async Task<AtsAnalysisResult> AnalyzeAtsCompatibilityAsync(Guid personId, CancellationToken ct = default)
    {
        var person = await _personRepo.GetFullProfileAsync(personId, ct);
        if (person == null)
            return new AtsAnalysisResult();

        var issues = new List<string>();
        var suggestions = new List<string>();
        var detectedKeywords = new List<string>();
        var missingKeywords = new List<string>();
        var score = 0;

        // 1. Resumo profissional (máx 20 pts)
        if (!string.IsNullOrWhiteSpace(person.ProfessionalSummary))
        {
            score += person.ProfessionalSummary.Length >= 100 ? 20 : 10;
            detectedKeywords.AddRange(ExtractKeywords(person.ProfessionalSummary));
        }
        else
        {
            issues.Add("Resumo profissional não preenchido");
            suggestions.Add("Adicione um resumo profissional de no mínimo 100 caracteres");
        }

        // 2. Experiências (máx 25 pts)
        if (person.Experiences.Count > 0)
        {
            var expScore = Math.Min(person.Experiences.Count * 8, 25);
            score += expScore;

            foreach (var exp in person.Experiences)
            {
                if (!string.IsNullOrWhiteSpace(exp.Description))
                {
                    detectedKeywords.AddRange(ExtractKeywords(exp.Description));
                }

                if (!exp.IsCurrent && exp.EndDate.HasValue && (DateTime.UtcNow - exp.EndDate.Value).TotalDays > 365)
                {
                    issues.Add($"Experiência antiga: {exp.Position} na {exp.CompanyName} (mais de 1 ano sem atualizar)");
                }
            }

            if (person.Experiences.All(e => string.IsNullOrWhiteSpace(e.Description) || e.Description.Length < 50))
            {
                issues.Add("Descrições de experiência muito curtas");
                suggestions.Add("Descreva suas responsabilidades e conquistas em cada experiência (mín. 50 caracteres)");
            }
        }
        else
        {
            issues.Add("Nenhuma experiência profissional cadastrada");
            suggestions.Add("Adicione pelo menos uma experiência profissional");
        }

        // 3. Formação (máx 15 pts)
        if (person.Educations.Count > 0)
        {
            score += Math.Min(person.Educations.Count * 5, 15);
        }
        else
        {
            issues.Add("Nenhuma formação acadêmica cadastrada");
            suggestions.Add("Adicione sua formação acadêmica");
        }

        // 4. Habilidades (máx 25 pts)
        var skillText = string.Join(" ", person.Skills.Select(s => s.Name.ToLowerInvariant()));
        if (person.Skills.Count > 0)
        {
            score += Math.Min(person.Skills.Count * 3, 25);

            foreach (var skill in person.Skills)
            {
                if (skill.IsPrimary)
                    detectedKeywords.Add(skill.Name.ToLowerInvariant());
            }
        }
        else
        {
            issues.Add("Nenhuma habilidade cadastrada");
            suggestions.Add("Adicione suas habilidades técnicas e comportamentais");
        }

        // 5. Keywords (máx 15 pts)
        var allText = BuildFullText(person).ToLowerInvariant();
        var techFound = TechKeywords.Where(k => allText.Contains(k)).ToList();
        var softFound = SoftSkills.Where(k => allText.Contains(k)).ToList();

        detectedKeywords.AddRange(techFound);
        detectedKeywords.AddRange(softFound);

        missingKeywords = TechKeywords.Where(k => !allText.Contains(k))
            .Take(5)
            .ToList();

        var keywordScore = Math.Min((techFound.Count + softFound.Count) * 2, 15);
        score += keywordScore;

        if (techFound.Count < 3)
        {
            suggestions.Add("Adicione mais palavras-chave técnicas relevantes para sua área");
        }

        // 6. Contato e dados pessoais (máx 10 pts)
        if (!string.IsNullOrWhiteSpace(person.Phone)) score += 3;
        if (!string.IsNullOrWhiteSpace(person.City)) score += 2;
        if (!string.IsNullOrWhiteSpace(person.State)) score += 2;
        if (!string.IsNullOrWhiteSpace(person.CurrentPosition)) score += 3;

        if (string.IsNullOrWhiteSpace(person.Phone))
        {
            issues.Add("Telefone não preenchido");
            suggestions.Add("Adicione um telefone para contato");
        }

        score = Math.Min(score, 100);

        _logger.LogInformation("📊 Análise ATS concluída para PersonId={PersonId}: Score={Score}", personId, score);

        return new AtsAnalysisResult
        {
            Score = score,
            Compatibility = CalculateCompatibility(score),
            Issues = issues.Distinct().ToList(),
            Suggestions = suggestions.Distinct().ToList(),
            DetectedKeywords = detectedKeywords.Distinct().ToList(),
            MissingKeywords = missingKeywords.Distinct().ToList()
        };
    }

    public async Task<IReadOnlyList<ResumeSuggestionResult>> GenerateSuggestionsAsync(Guid personId, CancellationToken ct = default)
    {
        var person = await _personRepo.GetFullProfileAsync(personId, ct);
        if (person == null)
            return new List<ResumeSuggestionResult>().AsReadOnly();

        var suggestions = new List<ResumeSuggestionResult>();

        // Experiência
        if (person.Experiences.Count == 0)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Experiência",
                Title = "Adicione experiências profissionais",
                Description = "Experiências profissionais são essenciais para mostrar sua trajetória. Adicione pelo menos uma experiência com descrição detalhada.",
                Priority = "high"
            });
        }
        else if (person.Experiences.Any(e => string.IsNullOrWhiteSpace(e.Description) || e.Description.Length < 50))
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Experiência",
                Title = "Detalhe suas experiências",
                Description = "Algumas experiências estão com descrição muito curta. Adicione detalhes sobre responsabilidades e resultados alcançados.",
                Priority = "high"
            });
        }

        // Formação
        if (person.Educations.Count == 0)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Formação",
                Title = "Adicione sua formação acadêmica",
                Description = "A formação acadêmica é importante para validar seu conhecimento técnico.",
                Priority = "high"
            });
        }

        // Habilidades
        if (person.Skills.Count < 5)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Habilidades",
                Title = "Adicione mais habilidades",
                Description = "Quanto mais habilidades relevantes, melhor seu currículo será classificado pelos sistemas ATS.",
                Priority = "medium"
            });
        }

        if (person.Skills.All(s => !s.IsPrimary))
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Habilidades",
                Title = "Destaque suas habilidades principais",
                Description = "Marque as habilidades mais relevantes como 'Principal' para destacá-las no currículo.",
                Priority = "medium"
            });
        }

        // Resumo
        if (string.IsNullOrWhiteSpace(person.ProfessionalSummary))
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Resumo",
                Title = "Adicione um resumo profissional",
                Description = "Um resumo profissional bem escrito aumenta significativamente o interesse dos recrutadores.",
                Priority = "high"
            });
        }
        else if (person.ProfessionalSummary.Length < 100)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Resumo",
                Title = "Amplie seu resumo profissional",
                Description = "Seu resumo profissional deve ter no mínimo 100 caracteres para ser mais eficaz.",
                Priority = "medium"
            });
        }

        // Idiomas
        if (person.Languages.Count == 0)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Idiomas",
                Title = "Adicione idiomas",
                Description = "Idiomas são diferenciais importantes no mercado de trabalho.",
                Priority = "low"
            });
        }

        // Certificações
        if (person.Certificates.Count == 0)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Certificações",
                Title = "Adicione certificações",
                Description = "Certificações comprovam seu conhecimento e dedicação à área.",
                Priority = "low"
            });
        }

        // Redes Sociais
        if (person.SocialNetworks.Count == 0)
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Redes Sociais",
                Title = "Adicione links profissionais",
                Description = "LinkedIn e outras redes sociais profissionais aumentam sua credibilidade.",
                Priority = "low"
            });
        }

        // Foto
        if (string.IsNullOrWhiteSpace(person.PhotoUrl))
        {
            suggestions.Add(new ResumeSuggestionResult
            {
                Category = "Perfil",
                Title = "Adicione uma foto profissional",
                Description = "Currículos com foto têm até 40% mais chances de serem visualizados.",
                Priority = "low"
            });
        }

        _logger.LogInformation("💡 Sugestões geradas para PersonId={PersonId}: {Count} sugestões", personId, suggestions.Count);

        return suggestions.AsReadOnly();
    }

    private static string BuildFullText(Person person)
    {
        var parts = new List<string>
        {
            person.ProfessionalSummary ?? ""
        };

        parts.AddRange(person.Experiences.Select(e =>
            $"{e.Position} {e.CompanyName} {e.Description}"));
        parts.AddRange(person.Educations.Select(e =>
            $"{e.Course} {e.Institution} {e.Description}"));
        parts.AddRange(person.Skills.Select(s => s.Name));

        return string.Join(" ", parts);
    }

    private static int CalculateCompatibility(int score) =>
        score switch
        {
            >= 80 => 90,
            >= 60 => 75,
            >= 40 => 55,
            >= 20 => 35,
            _ => 15
        };

    private static IEnumerable<string> ExtractKeywords(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;

        var lower = text.ToLowerInvariant();
        foreach (var keyword in TechKeywords.Concat(SoftSkills).Distinct())
        {
            if (lower.Contains(keyword))
                yield return keyword;
        }
    }
}
