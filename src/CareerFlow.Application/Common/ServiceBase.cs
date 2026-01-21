using AutoMapper;
using CareerFlow.Domain.Common;
using CareerFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareerFlow.Application.Common;

public abstract class ServiceBase
{
    protected readonly ApplicationDbContext _context;
    protected readonly IMapper _mapper;
    protected readonly ILogger _logger;

    protected ServiceBase(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    protected async Task<bool> EntityExistsAsync<T>(Guid id) where T : BaseEntity
    {
        return await _context.Set<T>().AnyAsync(e => e.Id == id);
    }

    protected async Task<T?> GetEntityByIdAsync<T>(Guid id) where T : BaseEntity
    {
        return await _context.Set<T>().FindAsync(id);
    }
}