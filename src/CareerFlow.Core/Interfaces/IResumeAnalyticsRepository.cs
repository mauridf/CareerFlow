using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface IResumeAnalyticsRepository : IRepository<ResumeAnalytics>
{
    Task<ResumeAnalytics?> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
