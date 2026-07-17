using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface ICertificateRepository : IRepository<Certificate>
{
    Task<IReadOnlyList<Certificate>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
