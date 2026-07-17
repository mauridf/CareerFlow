using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Interface específica para o repositório de Education.
/// </summary>
public interface IEducationRepository : IRepository<Education>
{
    Task<IReadOnlyList<Education>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
