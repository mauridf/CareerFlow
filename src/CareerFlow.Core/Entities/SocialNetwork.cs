using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa uma rede social associada ao perfil.
/// </summary>
public class SocialNetwork : Entity<Guid>
{
    /// <summary>ID do perfil associado</summary>
    public Guid PersonId { get; private set; }

    /// <summary>Perfil associado</summary>
    public Person? Person { get; private set; }

    /// <summary>Tipo da rede social</summary>
    public SocialNetworkType NetworkType { get; private set; }

    /// <summary>URL do perfil na rede social</summary>
    public string Url { get; private set; } = string.Empty;

    /// <summary>Ordem de exibição</summary>
    public int DisplayOrder { get; private set; }

    private SocialNetwork() { }

    /// <summary>
    /// Cria uma nova rede social
    /// </summary>
    public static SocialNetwork Create(Guid personId, SocialNetworkType type, string url, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL é obrigatória", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("URL inválida", nameof(url));

        return new SocialNetwork
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            NetworkType = type,
            Url = url.Trim(),
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Atualiza os dados da rede social
    /// </summary>
    public void Update(string url, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL é obrigatória", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out _))
            throw new ArgumentException("URL inválida", nameof(url));

        Url = url.Trim();
        DisplayOrder = displayOrder;
        MarkAsUpdated();
    }
}
