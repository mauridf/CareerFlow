using CareerFlow.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace CareerFlow.Domain.Common;

public static class DbContextExtensions
{
    public static DbSet<T> Set<T>(this IApplicationDbContext context) where T : BaseEntity
    {
        return ((DbContext)context).Set<T>();
    }
}