using CareerFlow.Core.Entities;

namespace CareerFlow.Core.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<Person?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Person?> GetFullProfileAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<Person?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<bool> SlugExistsAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Person>> GetAllWithBasicDetailsAsync(CancellationToken cancellationToken = default);
}
