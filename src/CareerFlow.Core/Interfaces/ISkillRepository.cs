using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Interface específica para o repositório de Skill.
/// </summary>
public interface ISkillRepository : IRepository<Skill>
{
    Task<IReadOnlyList<Skill>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid personId, string name, CancellationToken cancellationToken = default);
}
