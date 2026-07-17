using CareerFlow.Core.Entities;
using CareerFlow.Core.Events;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class ResumeSuggestionTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ShouldRaiseCreatedEvent()
    {
        var suggestion = ResumeSuggestion.Create(
            _personId, "experience", "Adicionar experiência",
            "Descrição da melhoria", "high");

        suggestion.Category.Should().Be("experience");
        suggestion.Title.Should().Be("Adicionar experiência");
        suggestion.Priority.Should().Be("high");
        suggestion.IsApplied.Should().BeFalse();
        suggestion.DomainEvents.Should().HaveCount(1);
        suggestion.DomainEvents.Should().AllBeOfType<ResumeSuggestionCreatedEvent>();
    }

    [Fact]
    public void MarkAsApplied_ShouldRaiseAppliedEvent()
    {
        var suggestion = ResumeSuggestion.Create(
            _personId, "skill", "Adicionar skill", null);

        suggestion.MarkAsApplied();

        suggestion.IsApplied.Should().BeTrue();
        suggestion.AppliedAt.Should().NotBeNull();
        suggestion.DomainEvents.Should().HaveCount(2);
        suggestion.DomainEvents.Should().ContainItemsAssignableTo<ResumeSuggestionAppliedEvent>();
    }
}
