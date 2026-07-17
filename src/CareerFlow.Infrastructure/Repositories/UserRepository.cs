using Microsoft.EntityFrameworkCore;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Data;

namespace CareerFlow.Infrastructure.Repositories;

/// <summary>
/// Repositório específico para a entidade User.
/// </summary>
public class UserRepository : GenericRepository<User>, IUserRepository
{
    public UserRepository(CareerFlowDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Busca usuário por email (apenas ativos e não deletados)
    /// </summary>
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u =>
                u.Email == email.ToLowerInvariant().Trim() &&
                u.DeletedAt == null,
                cancellationToken);
    }

    /// <summary>
    /// Busca usuário com Person incluído
    /// </summary>
    public async Task<User?> GetByIdWithPersonAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Person)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    /// <summary>
    /// Verifica se email já está em uso
    /// </summary>
    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(
            u => u.Email == email.ToLowerInvariant().Trim() && u.DeletedAt == null,
            cancellationToken);
    }
}
