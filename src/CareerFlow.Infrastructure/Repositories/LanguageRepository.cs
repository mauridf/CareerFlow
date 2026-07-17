using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
{
    public LanguageRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Language>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(l => l.PersonId == personId)
            .OrderBy(l => l.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(l => l.PersonId == personId, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Guid personId, string languageName, CancellationToken cancellationToken = default)
    {
        var normalized = languageName.Trim().ToLowerInvariant();
        return await _dbSet.AnyAsync(
            l => l.PersonId == personId && l.LanguageName.ToLower() == normalized,
            cancellationToken);
    }
}
