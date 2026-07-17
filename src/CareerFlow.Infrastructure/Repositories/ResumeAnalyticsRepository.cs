using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class ResumeAnalyticsRepository : GenericRepository<ResumeAnalytics>, IResumeAnalyticsRepository
{
    public ResumeAnalyticsRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<ResumeAnalytics?> GetByPersonIdAsync(Guid personId, CancellationToken ct = default)
        => await _dbSet.FirstOrDefaultAsync(a => a.PersonId == personId, ct);
}
