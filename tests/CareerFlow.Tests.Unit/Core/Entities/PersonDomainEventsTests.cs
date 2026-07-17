using CareerFlow.Core.Entities;
using CareerFlow.Core.Events;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class PersonDomainEventsTests
{
    [Fact]
    public void Share_ShouldRaiseResumeSharedEvent()
    {
        var user = User.Create("Test", "test@test.com", "hash");
        var person = Person.Create(user.Id);

        person.Share();

        person.IsPublic.Should().BeTrue();
        person.DomainEvents.Should().ContainItemsAssignableTo<ResumeSharedEvent>();
    }
}
