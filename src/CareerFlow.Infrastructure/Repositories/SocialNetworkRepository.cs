using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class SocialNetworkRepository : GenericRepository<SocialNetwork>, ISocialNetworkRepository
{
    public SocialNetworkRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<SocialNetwork>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.PersonId == personId)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(s => s.PersonId == personId, cancellationToken);
    }
}
