using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class SkillRepository : GenericRepository<Skill>
{
    public SkillRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Skill>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.PersonId == personId)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }
}
