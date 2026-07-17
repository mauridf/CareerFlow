using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Core.Events;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class EducationTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ShouldCreateEducation()
    {
        var education = Education.Create(
            _personId, "USP", "Ciência da Computação",
            EducationLevel.Graduation,
            new DateTime(2018, 1, 1), new DateTime(2022, 12, 31));

        education.Institution.Should().Be("USP");
        education.Course.Should().Be("Ciência da Computação");
        education.IsCurrent.Should().BeFalse();
        education.DomainEvents.Should().HaveCount(1);
        education.DomainEvents.Should().AllBeOfType<EducationCreatedEvent>();
    }

    [Fact]
    public void Create_WithoutEndDate_ShouldSetIsCurrent()
    {
        var education = Education.Create(
            _personId, "USP", "Ciência da Computação",
            EducationLevel.Graduation,
            new DateTime(2023, 1, 1), null);

        education.IsCurrent.Should().BeTrue();
        education.Status.Should().Be(EducationStatus.InProgress);
    }

    [Fact]
    public void Create_StartDateAfterEndDate_ShouldThrow()
    {
        var action = () => Education.Create(
            _personId, "USP", "CC", EducationLevel.Graduation,
            new DateTime(2022, 1, 1), new DateTime(2020, 1, 1));

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldRaiseUpdatedEvent()
    {
        var education = Education.Create(
            _personId, "USP", "CC", EducationLevel.Graduation,
            new DateTime(2018, 1, 1), new DateTime(2022, 12, 31));

        education.Update("FGV", "ADM", EducationLevel.Master,
            new DateTime(2023, 1, 1), null, EducationStatus.InProgress);

        education.Institution.Should().Be("FGV");
        education.DomainEvents.Should().HaveCount(2);
        education.DomainEvents.Should().ContainItemsAssignableTo<EducationUpdatedEvent>();
    }

    [Fact]
    public void Create_EmptyInstitution_ShouldThrow()
    {
        var action = () => Education.Create(
            _personId, "", "CC", EducationLevel.Graduation,
            DateTime.Now, null);

        action.Should().Throw<ArgumentException>();
    }
}
