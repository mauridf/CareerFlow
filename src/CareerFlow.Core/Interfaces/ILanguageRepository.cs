using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface ILanguageRepository : IRepository<Language>
{
    Task<IReadOnlyList<Language>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(Guid personId, string languageName, CancellationToken cancellationToken = default);
}
