using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface ISocialNetworkRepository : IRepository<SocialNetwork>
{
    Task<IReadOnlyList<SocialNetwork>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
