using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para a entidade Person.
/// </summary>
public class PersonRepository : GenericRepository<Person>, IPersonRepository
{
    public PersonRepository(CareerFlowDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca perfil por ID do usuário
    /// </summary>
    public async Task<Person?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
    }

    /// <summary>
    /// Busca perfil completo com todos os relacionamentos
    /// </summary>
    public async Task<Person?> GetFullProfileAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.SocialNetworks.OrderBy(s => s.DisplayOrder))
            .Include(p => p.Skills.OrderBy(s => s.DisplayOrder))
            .Include(p => p.Experiences.OrderByDescending(e => e.StartDate))
            .Include(p => p.Educations.OrderByDescending(e => e.StartDate))
            .Include(p => p.Certificates.OrderByDescending(c => c.IssueDate))
            .Include(p => p.Languages.OrderBy(l => l.DisplayOrder))
            .FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);
    }

    /// <summary>
    /// Busca perfil público por slug
    /// </summary>
    public async Task<Person?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(p => p.User)
            .Include(p => p.SocialNetworks.OrderBy(s => s.DisplayOrder))
            .Include(p => p.Skills.OrderBy(s => s.DisplayOrder))
            .Include(p => p.Experiences.OrderByDescending(e => e.StartDate))
            .Include(p => p.Educations.OrderByDescending(e => e.StartDate))
            .Include(p => p.Certificates.OrderByDescending(c => c.IssueDate))
            .Include(p => p.Languages.OrderBy(l => l.DisplayOrder))
            .FirstOrDefaultAsync(p =>
                p.ResumeSlug == slug &&
                p.IsPublic &&
                p.User != null &&
                p.User.IsActive,
                cancellationToken);
    }

    /// <summary>
    /// Verifica se slug já está em uso
    /// </summary>
    public async Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.ResumeSlug == slug, cancellationToken);
    }
}
