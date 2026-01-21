using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CareerFlow.Infrastructure.Data;

public static class SeedData
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Users.AnyAsync())
            return; // Database já foi seedado

        // Criar usuário de exemplo
        var user = new User
        {
            Name = "João Silva",
            Email = "joao.silva@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Senha@123"),
            Phone = "(11) 99999-9999",
            City = "São Paulo",
            State = "SP",
            PhotoPath = null
        };

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        // Resumo Profissional
        var summary = new ProfessionalSummary
        {
            UserId = user.Id,
            Summary = "Desenvolvedor Backend com 5 anos de experiência em .NET, C# e arquiteturas de microserviços. Especializado em desenvolvimento de APIs RESTful e integração de sistemas."
        };
        await context.ProfessionalSummaries.AddAsync(summary);

        // Redes Sociais
        var socialMedias = new List<SocialMedia>
        {
            new() { UserId = user.Id, Platform = "LinkedIn", Url = "https://linkedin.com/in/joaosilva" },
            new() { UserId = user.Id, Platform = "GitHub", Url = "https://github.com/joaosilva" }
        };
        await context.SocialMedias.AddRangeAsync(socialMedias);

        // Habilidades
        var skills = new List<Skill>
        {
            new() { UserId = user.Id, Name = "C#", Type = SkillType.BACKEND, Level = SkillLevel.ADVANCED },
            new() { UserId = user.Id, Name = ".NET 8", Type = SkillType.BACKEND, Level = SkillLevel.ADVANCED },
            new() { UserId = user.Id, Name = "PostgreSQL", Type = SkillType.DATABASE, Level = SkillLevel.INTERMEDIATE },
            new() { UserId = user.Id, Name = "React", Type = SkillType.FRONTEND, Level = SkillLevel.BASIC }
        };
        await context.Skills.AddRangeAsync(skills);

        // Formação Acadêmica
        var academics = new List<AcademicBackground>
        {
            new()
            {
                UserId = user.Id,
                Institution = "Universidade de São Paulo",
                CourseName = "Ciência da Computação",
                Level = EducationLevel.GRADUATION,
                StartDate = new DateTime(2015, 3, 1),
                EndDate = new DateTime(2019, 12, 15)
            }
        };
        await context.AcademicBackgrounds.AddRangeAsync(academics);

        // Certificados
        var certificates = new List<Certificate>
        {
            new()
            {
                UserId = user.Id,
                Name = "Microsoft Certified: Azure Developer Associate",
                Description = "Certificação oficial Microsoft para desenvolvimento em Azure",
                StartDate = new DateTime(2022, 5, 1),
                EndDate = new DateTime(2024, 5, 1)
            }
        };
        await context.Certificates.AddRangeAsync(certificates);

        // Idiomas
        var languages = new List<Language>
        {
            new() { UserId = user.Id, Name = "Português", Level = LanguageLevel.FLUENT },
            new() { UserId = user.Id, Name = "Inglês", Level = LanguageLevel.ADVANCED },
            new() { UserId = user.Id, Name = "Espanhol", Level = LanguageLevel.INTERMEDIATE }
        };
        await context.Languages.AddRangeAsync(languages);

        // Experiência Profissional
        var experience = new ProfessionalExperience
        {
            UserId = user.Id,
            Company = "Tech Solutions Ltda",
            Position = "Desenvolvedor Backend Pleno",
            StartDate = new DateTime(2020, 1, 1),
            EndDate = null, // Trabalho atual
            Responsibilities = "Desenvolvimento de APIs RESTful, manutenção de microsserviços, implementação de testes unitários.",
            IsPaid = true
        };
        await context.ProfessionalExperiences.AddAsync(experience);

        await context.SaveChangesAsync();

        // Relacionar habilidades com experiência
        var skillExperience = new SkillExperience
        {
            SkillId = skills[0].Id, // C#
            ProfessionalExperienceId = experience.Id
        };
        await context.SkillExperiences.AddAsync(skillExperience);

        // Adicionar mais relacionamentos se quiser
        var skillExperience2 = new SkillExperience
        {
            SkillId = skills[1].Id, // .NET 8
            ProfessionalExperienceId = experience.Id
        };
        await context.SkillExperiences.AddAsync(skillExperience2);

        await context.SaveChangesAsync();
    }
}