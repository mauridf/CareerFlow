using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class ExperienceRepository : GenericRepository<Experience>
{
    public ExperienceRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Experience>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(e => e.PersonId == personId)
            .OrderByDescending(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }
}
