using CareerFlow.Core.Entities;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class PersonTests
{
    [Fact]
    public void Create_ShouldCreatePerson()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var person = Person.Create(userId);

        // Assert
        person.UserId.Should().Be(userId);
        person.IsPublic.Should().BeTrue();
        person.DomainEvents.Should().HaveCount(1);
    }

    [Fact]
    public void CalculateCompletionPercentage_EmptyProfile_ShouldReturnZero()
    {
        // Arrange
        var person = Person.Create(Guid.NewGuid());

        // Act
        var percentage = person.CalculateCompletionPercentage();

        // Assert
        percentage.Should().Be(0);
    }

    [Fact]
    public void CalculateCompletionPercentage_FullProfile_ShouldReturn100()
    {
        // Arrange
        var person = Person.Create(Guid.NewGuid());
        person.UpdatePersonalInfo("(11) 99999-9999", "São Paulo", "SP",
            new DateTime(1990, 1, 1), "Resumo profissional com mais de cem caracteres. " +
            "Desenvolvedor experiente em .NET e cloud computing com vasta experiência em projetos corporativos.");
        person.UpdatePhoto("http://foto.com/perfil.jpg");
        person.UpdateCurrentProfession("Tech Lead", "TechCorp");

        // Act
        var percentage = person.CalculateCompletionPercentage();

        // Assert
        percentage.Should().Be(100);
    }

    [Fact]
    public void CanGenerateResume_IncompleteProfile_ShouldReturnFalse()
    {
        // Arrange
        var person = Person.Create(Guid.NewGuid());

        // Act
        var canGenerate = person.CanGenerateResume();

        // Assert
        canGenerate.Should().BeFalse();
    }
}
