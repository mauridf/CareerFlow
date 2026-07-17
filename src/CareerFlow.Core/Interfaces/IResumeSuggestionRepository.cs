using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface IResumeSuggestionRepository : IRepository<ResumeSuggestion>
{
    Task<IReadOnlyList<ResumeSuggestion>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
