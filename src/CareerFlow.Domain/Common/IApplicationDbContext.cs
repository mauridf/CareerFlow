using CareerFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CareerFlow.Domain.Common;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<ProfessionalSummary> ProfessionalSummaries { get; }
    DbSet<SocialMedia> SocialMedias { get; }
    DbSet<Skill> Skills { get; }
    DbSet<AcademicBackground> AcademicBackgrounds { get; }
    DbSet<Certificate> Certificates { get; }
    DbSet<Language> Languages { get; }
    DbSet<ProfessionalExperience> ProfessionalExperiences { get; }
    DbSet<SkillExperience> SkillExperiences { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
