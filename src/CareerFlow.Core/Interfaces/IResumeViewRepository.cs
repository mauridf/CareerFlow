using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface IResumeViewRepository : IRepository<ResumeView>
{
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountUniqueByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ResumeView>> GetByPersonIdAsync(Guid personId, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountPdfDownloadsAsync(Guid personId, CancellationToken cancellationToken = default);
}
