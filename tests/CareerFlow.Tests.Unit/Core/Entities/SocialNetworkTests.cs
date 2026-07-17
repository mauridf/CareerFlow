using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;
using CareerFlow.Core.Exceptions;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class SocialNetworkTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ShouldCreateSocialNetwork()
    {
        var social = SocialNetwork.Create(
            _personId, SocialNetworkType.LinkedIn,
            "https://linkedin.com/in/test");

        social.NetworkType.Should().Be(SocialNetworkType.LinkedIn);
        social.Url.Should().Be("https://linkedin.com/in/test");
        social.DomainEvents.Should().HaveCount(1);
        social.DomainEvents.Should().AllBeOfType<SocialNetworkCreatedEvent>();
    }

    [Fact]
    public void Create_EmptyUrl_ShouldThrow()
    {
        var action = () => SocialNetwork.Create(
            _personId, SocialNetworkType.GitHub, "");

        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_ShouldRaiseUpdatedEvent()
    {
        var social = SocialNetwork.Create(
            _personId, SocialNetworkType.LinkedIn,
            "https://linkedin.com/in/test");

        social.Update(SocialNetworkType.GitHub,
            "https://github.com/test", 1);

        social.NetworkType.Should().Be(SocialNetworkType.GitHub);
        social.DomainEvents.Should().HaveCount(2);
        social.DomainEvents.Should().ContainItemsAssignableTo<SocialNetworkUpdatedEvent>();
    }
}
