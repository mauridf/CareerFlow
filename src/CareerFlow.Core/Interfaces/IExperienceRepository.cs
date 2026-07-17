using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Interface específica para o repositório de Experience.
/// </summary>
public interface IExperienceRepository : IRepository<Experience>
{
    Task<IReadOnlyList<Experience>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
}
