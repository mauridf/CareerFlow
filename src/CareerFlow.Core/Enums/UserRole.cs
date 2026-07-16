namespace CareerFlow.Core.Enums;

/// <summary>
/// Papéis de usuário no sistema.
/// Define permissões e acesso a funcionalidades.
/// </summary>
public enum UserRole
{
    /// <summary>Usuário comum (plano free)</summary>
    User = 0,

    /// <summary>Usuário premium (plano pago)</summary>
    PremiumUser = 1,

    /// <summary>Administrador do sistema</summary>
    Admin = 2
}

/// <summary>
/// Extensões para o enum UserRole
/// </summary>
public static class UserRoleExtensions
{
    /// <summary>
    /// Retorna o nome amigável do papel
    /// </summary>
    public static string GetDisplayName(this UserRole role)
    {
        return role switch
        {
            UserRole.User => "Usuário",
            UserRole.PremiumUser => "Usuário Premium",
            UserRole.Admin => "Administrador",
            _ => "Desconhecido"
        };
    }

    /// <summary>
    /// Verifica se o papel tem acesso administrativo
    /// </summary>
    public static bool IsAdmin(this UserRole role)
    {
        return role == UserRole.Admin;
    }

    /// <summary>
    /// Verifica se o papel tem acesso premium
    /// </summary>
    public static bool IsPremiumOrAbove(this UserRole role)
    {
        return role == UserRole.PremiumUser || role == UserRole.Admin;
    }
}
