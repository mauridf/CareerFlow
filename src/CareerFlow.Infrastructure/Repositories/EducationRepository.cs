using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class EducationRepository : GenericRepository<Education>, IEducationRepository
{
    public EducationRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Education>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.PersonId == personId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(e => e.PersonId == personId, cancellationToken);
    }
}
