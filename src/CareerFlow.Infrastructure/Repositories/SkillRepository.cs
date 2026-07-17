using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class SkillRepository : GenericRepository<Skill>, ISkillRepository
{
    public SkillRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Skill>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.PersonId == personId)
            .OrderBy(s => s.DisplayOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(Guid personId, string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.Trim().ToLowerInvariant();
        return await _dbSet.AnyAsync(
            s => s.PersonId == personId && s.Name.ToLower() == normalizedName,
            cancellationToken);
    }
}
