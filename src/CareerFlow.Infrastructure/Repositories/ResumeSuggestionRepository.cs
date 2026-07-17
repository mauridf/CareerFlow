using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class ResumeSuggestionRepository : GenericRepository<ResumeSuggestion>, IResumeSuggestionRepository
{
    public ResumeSuggestionRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<ResumeSuggestion>> GetByPersonIdAsync(Guid personId, CancellationToken ct = default)
    {
        return await _dbSet
            .Where(s => s.PersonId == personId)
            .OrderByDescending(s => s.Priority == "high" ? 0 : s.Priority == "medium" ? 1 : 2)
            .ThenByDescending(s => s.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<int> CountByPersonIdAsync(Guid personId, CancellationToken ct = default)
    {
        return await _dbSet.CountAsync(s => s.PersonId == personId, ct);
    }
}
