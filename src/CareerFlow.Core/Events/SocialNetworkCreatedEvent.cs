using CareerFlow.Core.Enums;

namespace CareerFlow.Core.Events;

public sealed class SocialNetworkCreatedEvent : DomainEvent
{
    public Guid SocialNetworkId { get; }
    public Guid PersonId { get; }
    public SocialNetworkType NetworkType { get; }

    public SocialNetworkCreatedEvent(Guid socialNetworkId, Guid personId, SocialNetworkType networkType)
    {
        SocialNetworkId = socialNetworkId;
        PersonId = personId;
        NetworkType = networkType;
    }
}
