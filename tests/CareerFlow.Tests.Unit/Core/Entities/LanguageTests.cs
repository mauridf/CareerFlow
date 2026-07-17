using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class LanguageTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ShouldCreateLanguage()
    {
        var lang = Language.Create(_personId, "Inglês", LanguageLevel.C1);

        lang.LanguageName.Should().Be("Inglês");
        lang.ProficiencyLevel.Should().Be(LanguageLevel.C1);
        lang.IsNative.Should().BeFalse();
        lang.DomainEvents.Should().HaveCount(1);
        lang.DomainEvents.Should().AllBeOfType<LanguageCreatedEvent>();
    }

    [Fact]
    public void Create_EmptyName_ShouldThrow()
    {
        var action = () => Language.Create(_personId, "", LanguageLevel.A1);
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldRaiseUpdatedEvent()
    {
        var lang = Language.Create(_personId, "Inglês", LanguageLevel.B1);

        lang.Update("Inglês", LanguageLevel.C1, false);

        lang.ProficiencyLevel.Should().Be(LanguageLevel.C1);
        lang.DomainEvents.Should().HaveCount(2);
        lang.DomainEvents.Should().ContainItemsAssignableTo<LanguageUpdatedEvent>();
    }
}
