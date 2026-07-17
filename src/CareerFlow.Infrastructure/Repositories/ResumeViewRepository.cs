using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

public class ResumeViewRepository : GenericRepository<ResumeView>, IResumeViewRepository
{
    public ResumeViewRepository(CareerFlowDbContext context) : base(context) { }

    public async Task<int> CountByPersonIdAsync(Guid personId, CancellationToken ct = default)
        => await _dbSet.CountAsync(v => v.PersonId == personId, ct);

    public async Task<int> CountUniqueByPersonIdAsync(Guid personId, CancellationToken ct = default)
        => await _dbSet.Where(v => v.PersonId == personId).Select(v => v.IpAddress).Distinct().CountAsync(ct);

    public async Task<IReadOnlyList<ResumeView>> GetByPersonIdAsync(Guid personId, int page, int pageSize, CancellationToken ct = default)
        => await _dbSet.Where(v => v.PersonId == personId)
            .OrderByDescending(v => v.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> CountPdfDownloadsAsync(Guid personId, CancellationToken ct = default)
        => await _dbSet.CountAsync(v => v.PersonId == personId && v.PdfDownloaded, ct);
}
