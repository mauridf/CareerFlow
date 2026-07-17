using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;
using CareerFlow.Core.Exceptions;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class SkillTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ShouldCreateSkill()
    {
        var skill = Skill.Create(_personId, "C#", SkillCategory.Backend, ProficiencyLevel.Advanced);

        skill.Name.Should().Be("C#");
        skill.Category.Should().Be(SkillCategory.Backend);
        skill.ProficiencyLevel.Should().Be(ProficiencyLevel.Advanced);
        skill.DomainEvents.Should().HaveCount(1);
        skill.DomainEvents.Should().AllBeOfType<SkillCreatedEvent>();
    }

    [Fact]
    public void Create_EmptyName_ShouldThrow()
    {
        var action = () => Skill.Create(_personId, "", SkillCategory.Other);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Create_NameTooLong_ShouldThrow()
    {
        var action = () => Skill.Create(_personId, new string('a', 101), SkillCategory.Other);
        action.Should().Throw<DomainException>();
    }

    [Fact]
    public void Update_ShouldRaiseUpdatedEvent()
    {
        var skill = Skill.Create(_personId, "C#", SkillCategory.Backend);
        skill.Update("F#", SkillCategory.Backend, ProficiencyLevel.Intermediate, false, 0);

        skill.Name.Should().Be("F#");
        skill.DomainEvents.Should().HaveCount(2);
        skill.DomainEvents.Should().ContainItemsAssignableTo<SkillUpdatedEvent>();
    }

    [Fact]
    public void TogglePrimary_ShouldToggle()
    {
        var skill = Skill.Create(_personId, "C#", SkillCategory.Backend);
        skill.IsPrimary.Should().BeFalse();

        skill.TogglePrimary();
        skill.IsPrimary.Should().BeTrue();
    }

    [Fact]
    public void GetScore_ShouldReturnCorrectValue()
    {
        var skill = Skill.Create(_personId, "C#", SkillCategory.Backend, ProficiencyLevel.Advanced);
        skill.GetScore().Should().Be(75);
    }
}
