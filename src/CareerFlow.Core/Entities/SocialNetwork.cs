using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;
using CareerFlow.Core.ValueObjects;

namespace CareerFlow.Core.Entities;

/// <summary>
/// Entidade que representa uma rede social associada ao perfil.
/// </summary>
public class SocialNetwork : AggregateRoot<Guid>
{
    public Guid PersonId { get; private set; }
    public Person? Person { get; private set; }

    public SocialNetworkType NetworkType { get; private set; }
    public string Url { get; private set; } = string.Empty;
    public int DisplayOrder { get; private set; }

    private SocialNetwork() { }

    public static SocialNetwork Create(Guid personId, SocialNetworkType type, string url, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL é obrigatória");

        // Valida a URL usando o Value Object
        var urlVO = new ValueObjects.Url(url);

        var socialNetwork = new SocialNetwork
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            NetworkType = type,
            Url = urlVO.Value,
            DisplayOrder = displayOrder,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        socialNetwork.AddDomainEvent(new SocialNetworkCreatedEvent(socialNetwork.Id, personId, type));

        return socialNetwork;
    }

    public void Update(SocialNetworkType type, string url, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new DomainException("URL é obrigatória");

        var urlVO = new ValueObjects.Url(url);

        NetworkType = type;
        Url = urlVO.Value;
        DisplayOrder = displayOrder;
        MarkAsUpdated();
        AddDomainEvent(new SocialNetworkUpdatedEvent(Id, PersonId));
    }
}
