using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Exceptions;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class ExperienceTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ShouldCreateExperience()
    {
        // Act
        var exp = Experience.Create(
            _personId, "TechCorp", "Developer",
            new DateTime(2020, 1, 1), null,
            "Desenvolvimento de APIs REST com .NET Core, microsserviços, Docker e muito mais habilidades");

        // Assert
        exp.CompanyName.Should().Be("TechCorp");
        exp.Position.Should().Be("Developer");
        exp.IsCurrent.Should().BeTrue();
        exp.DomainEvents.Should().HaveCount(1); // ExperienceCreatedEvent
    }

    [Fact]
    public void Create_DescriptionLessThan50Chars_ShouldThrowException()
    {
        // Act
        var action = () => Experience.Create(
            _personId, "TechCorp", "Developer",
            DateTime.Now, null, "Curta");

        // Assert
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void GetFormattedDuration_ShouldReturnCorrectFormat()
    {
        // Arrange
        var exp = Experience.Create(
            _personId, "TechCorp", "Developer",
            new DateTime(2020, 1, 1), new DateTime(2022, 12, 31),
            "Desenvolvimento de APIs REST com .NET Core, microsserviços, Docker e Kubernetes");

        // Act
        var duration = exp.GetFormattedDuration();

        // Assert
        duration.Should().Be("2 anos e 11 meses");
    }

    [Fact]
    public void AddSkill_ShouldAddToSkillsUsed()
    {
        var exp = Experience.Create(
            _personId, "TechCorp", "Developer",
            DateTime.Now, null,
            "Descrição válida com mais de cinquenta caracteres valida para teste de adicao");
        var skillId = Guid.NewGuid();

        exp.AddSkill(skillId);

        exp.SkillsUsed.Should().Contain(skillId);
    }
}
