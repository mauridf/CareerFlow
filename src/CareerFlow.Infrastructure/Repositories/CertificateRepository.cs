using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class CertificateRepository : GenericRepository<Certificate>, ICertificateRepository
{
    public CertificateRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Certificate>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.PersonId == personId)
            .OrderByDescending(c => c.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByPersonIdAsync(Guid personId, CancellationToken cancellationToken = default)
    {
        return await _dbSet.CountAsync(c => c.PersonId == personId, cancellationToken);
    }
}
