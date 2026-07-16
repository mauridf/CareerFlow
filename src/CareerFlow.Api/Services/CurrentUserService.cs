using System.Security.Claims;
using CareerFlow.Core.Interfaces;
using CareerFlow.Infrastructure.Repositories;

namespace CareerFlow.Api.Services;

/// <summary>
/// Serviço que obtém informações do usuário autenticado a partir do HttpContext.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly PersonRepository _personRepository;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor,
        PersonRepository personRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _personRepository = personRepository;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userIdClaim != null ? Guid.Parse(userIdClaim) : null;
        }
    }

    public string? Email => User?.FindFirst(ClaimTypes.Email)?.Value;

    public string? Name => User?.FindFirst(ClaimTypes.GivenName)?.Value;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public bool IsAdmin => Role == "Admin";

    public bool IsPremium
    {
        get
        {
            var isPremiumClaim = User?.FindFirst("isPremium")?.Value;
            return isPremiumClaim != null && bool.Parse(isPremiumClaim);
        }
    }

    /// <summary>
    /// Obtém o ID do perfil (Person) do usuário autenticado
    /// </summary>
    public async Task<Guid> GetPersonIdAsync(CancellationToken cancellationToken = default)
    {
        if (!UserId.HasValue)
            throw new UnauthorizedAccessException("Usuário não autenticado");

        var person = await _personRepository.GetByUserIdAsync(UserId.Value, cancellationToken);

        if (person == null)
            throw new InvalidOperationException($"Perfil não encontrado para o usuário {UserId}");

        return person.Id;
    }
}
