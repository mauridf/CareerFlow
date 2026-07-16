namespace CareerFlow.Core.Enums;

/// <summary>
/// Tipos de redes sociais suportadas.
/// </summary>
public enum SocialNetworkType
{
    /// <summary>LinkedIn</summary>
    LinkedIn = 0,

    /// <summary>GitHub</summary>
    GitHub = 1,

    /// <summary>GitLab</summary>
    GitLab = 2,

    /// <summary>Instagram</summary>
    Instagram = 3,

    /// <summary>Facebook</summary>
    Facebook = 4,

    /// <summary>Twitter/X</summary>
    Twitter = 5,

    /// <summary>YouTube</summary>
    YouTube = 6,

    /// <summary>Portfólio pessoal</summary>
    Portfolio = 7,

    /// <summary>Outra rede social</summary>
    Other = 8
}

public static class SocialNetworkTypeExtensions
{
    private static readonly Dictionary<SocialNetworkType, string> DisplayNames = new()
    {
        { SocialNetworkType.LinkedIn, "LinkedIn" },
        { SocialNetworkType.GitHub, "GitHub" },
        { SocialNetworkType.GitLab, "GitLab" },
        { SocialNetworkType.Instagram, "Instagram" },
        { SocialNetworkType.Facebook, "Facebook" },
        { SocialNetworkType.Twitter, "Twitter/X" },
        { SocialNetworkType.YouTube, "YouTube" },
        { SocialNetworkType.Portfolio, "Portfólio" },
        { SocialNetworkType.Other, "Outro" }
    };

    private static readonly Dictionary<SocialNetworkType, string> BaseUrls = new()
    {
        { SocialNetworkType.LinkedIn, "https://linkedin.com/in/" },
        { SocialNetworkType.GitHub, "https://github.com/" },
        { SocialNetworkType.GitLab, "https://gitlab.com/" },
        { SocialNetworkType.Instagram, "https://instagram.com/" },
        { SocialNetworkType.Facebook, "https://facebook.com/" },
        { SocialNetworkType.Twitter, "https://twitter.com/" },
        { SocialNetworkType.YouTube, "https://youtube.com/@" }
    };

    /// <summary>
    /// Retorna o nome de exibição da rede social
    /// </summary>
    public static string GetDisplayName(this SocialNetworkType type)
    {
        return DisplayNames.TryGetValue(type, out var name) ? name : type.ToString();
    }

    /// <summary>
    /// Retorna a URL base da rede social (se aplicável)
    /// </summary>
    public static string? GetBaseUrl(this SocialNetworkType type)
    {
        return BaseUrls.TryGetValue(type, out var url) ? url : null;
    }

    /// <summary>
    /// Verifica se a rede social é profissional
    /// </summary>
    public static bool IsProfessional(this SocialNetworkType type)
    {
        return type == SocialNetworkType.LinkedIn ||
               type == SocialNetworkType.GitHub ||
               type == SocialNetworkType.GitLab ||
               type == SocialNetworkType.Portfolio;
    }
}
