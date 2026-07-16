namespace CareerFlow.Core.Interfaces;

/// <summary>
/// Serviço para obter informações do usuário atual autenticado.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>ID do usuário autenticado</summary>
    Guid? UserId { get; }

    /// <summary>Email do usuário autenticado</summary>
    string? Email { get; }

    /// <summary>Nome do usuário autenticado</summary>
    string? Name { get; }

    /// <summary>Role do usuário autenticado</summary>
    string? Role { get; }

    /// <summary>Se o usuário está autenticado</summary>
    bool IsAuthenticated { get; }

    /// <summary>Se o usuário é admin</summary>
    bool IsAdmin { get; }

    /// <summary>Se o usuário é premium</summary>
    bool IsPremium { get; }

    /// <summary>ID do perfil (Person) do usuário</summary>
    Task<Guid> GetPersonIdAsync(CancellationToken cancellationToken = default);
}
