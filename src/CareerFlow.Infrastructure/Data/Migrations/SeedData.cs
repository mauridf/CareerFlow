using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;

namespace CareerFlow.Infrastructure.Data.Migrations;

/// <summary>
/// Dados iniciais (seed) para o banco de dados.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Aplica os dados iniciais
    /// </summary>
    public static async Task SeedAsync(CareerFlowDbContext context)
    {
        // Verifica se já existem dados
        if (await context.Users.AnyAsync())
            return;

        // ============================================
        // Admin padrão
        // ============================================
        var adminUser = User.Create(
            "Administrador CareerFlow",
            "admin@careerflow.com",
            "$2a$12$LJ3m4ys3Lk0TSwHCpNqrEOXQmTqPQVKGCs8BZrWYQpHmKvJL7Fz6u" // "Admin@123"
        );
        adminUser.SetId(Guid.Parse("a0000000-0000-0000-0000-000000000001"));
        adminUser.SetRole(UserRole.Admin);
        adminUser.VerifyEmail();
        adminUser.Activate();

        context.Users.Add(adminUser);

        // ============================================
        // Usuário de teste premium
        // ============================================
        var premiumUser = User.Create(
            "João Silva Premium",
            "joao.premium@email.com",
            "$2a$12$LJ3m4ys3Lk0TSwHCpNqrEOXQmTqPQVKGCs8BZrWYQpHmKvJL7Fz6u" // "Teste@123"
        );
        premiumUser.SetId(Guid.Parse("b0000000-0000-0000-0000-000000000001"));
        premiumUser.VerifyEmail();
        premiumUser.Activate();
        premiumUser.ActivatePremium(DateTime.UtcNow.AddYears(1));

        context.Users.Add(premiumUser);

        // Perfil do usuário premium
        var person = Person.Create(premiumUser.Id);
        person.SetId(Guid.Parse("c0000000-0000-0000-0000-000000000001"));
        person.UpdatePersonalInfo(
            "(11) 98765-4321",
            "São Paulo",
            "SP",
            new DateTime(1990, 5, 15),
            "Desenvolvedor Full Stack com mais de 8 anos de experiência em desenvolvimento de software. " +
            "Especialista em arquitetura de microsserviços, .NET Core e React. " +
            "Experiência em liderança técnica e mentoria de equipes."
        );
        person.UpdateCurrentProfession("Tech Lead", "TechCorp Brasil");
        person.SetResumeSlug("joao-silva-premium");
        person.SetPublic(true);

        context.Persons.Add(person);

        // Habilidades
        var skills = new List<Skill>
        {
            Skill.Create(person.Id, "C# / .NET", SkillCategory.Backend, ProficiencyLevel.Expert, true, 0),
            Skill.Create(person.Id, "ASP.NET Core", SkillCategory.Backend, ProficiencyLevel.Advanced, false, 1),
            Skill.Create(person.Id, "React", SkillCategory.Frontend, ProficiencyLevel.Advanced, true, 2),
            Skill.Create(person.Id, "PostgreSQL", SkillCategory.Database, ProficiencyLevel.Advanced, false, 3),
            Skill.Create(person.Id, "Docker", SkillCategory.CloudDevOps, ProficiencyLevel.Intermediate, false, 4),
            Skill.Create(person.Id, "AWS", SkillCategory.CloudDevOps, ProficiencyLevel.Intermediate, false, 5),
            Skill.Create(person.Id, "Liderança Técnica", SkillCategory.SoftSkills, ProficiencyLevel.Advanced, false, 6)
        };

        context.Skills.AddRange(skills);

        // Experiências
        var experiences = new List<Experience>
        {
            Experience.Create(
                person.Id, "TechCorp Brasil", "Tech Lead",
                new DateTime(2022, 1, 1), null,
                "Liderança técnica de equipe de 8 desenvolvedores. " +
                "Arquitetura e implementação de microsserviços em .NET. " +
                "Migração de sistema legado para cloud AWS. " +
                "Redução de 40% no tempo de deploy com CI/CD.",
                employmentType: EmploymentType.FullTime,
                city: "São Paulo", state: "SP",
                displayOrder: 0
            ),
            Experience.Create(
                person.Id, "Digital Solutions Ltda", "Desenvolvedor Sênior",
                new DateTime(2019, 3, 1), new DateTime(2021, 12, 31),
                "Desenvolvimento de APIs REST com ASP.NET Core. " +
                "Implementação de autenticação OAuth2 e JWT. " +
                "Otimização de queries PostgreSQL resultando em 60% de melhoria.",
                employmentType: EmploymentType.FullTime,
                city: "São Paulo", state: "SP",
                displayOrder: 1
            )
        };

        context.Experiences.AddRange(experiences);

        // Formação
        var educations = new List<Education>
        {
            Education.Create(
                person.Id, "Universidade de São Paulo (USP)",
                "Ciência da Computação",
                EducationLevel.Graduation,
                new DateTime(2010, 1, 1), new DateTime(2014, 12, 31),
                EducationStatus.Completed,
                grade: "8.5",
                displayOrder: 0
            )
        };

        context.Educations.AddRange(educations);

        // Analytics
        var analytics = ResumeAnalytics.Create(person.Id);
        analytics.Publish();
        context.ResumeAnalytics.Add(analytics);

        await context.SaveChangesAsync();

        Console.WriteLine("✅ Dados iniciais (seed) aplicados com sucesso!");
        Console.WriteLine("   Admin: admin@careerflow.com / Admin@123");
        Console.WriteLine("   Premium: joao.premium@email.com / Teste@123");
    }
}
