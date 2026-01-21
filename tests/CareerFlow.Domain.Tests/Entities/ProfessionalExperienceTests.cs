using CareerFlow.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Domain.Tests.Entities;

public class ProfessionalExperienceTests
{
    [Fact]
    public void ProfessionalExperience_IsCurrent_WhenNoEndDate_ShouldReturnTrue()
    {
        // Arrange
        var experience = new ProfessionalExperience
        {
            StartDate = new DateTime(2020, 1, 1),
            EndDate = null
        };

        // Act & Assert
        experience.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public void ProfessionalExperience_IsCurrent_WhenEndDateInFuture_ShouldReturnTrue()
    {
        // Arrange
        var experience = new ProfessionalExperience
        {
            StartDate = new DateTime(2020, 1, 1),
            EndDate = DateTime.UtcNow.AddMonths(1)
        };

        // Act & Assert
        experience.IsCurrent.Should().BeTrue();
    }

    [Fact]
    public void ProfessionalExperience_IsCurrent_WhenEndDateInPast_ShouldReturnFalse()
    {
        // Arrange
        var experience = new ProfessionalExperience
        {
            StartDate = new DateTime(2020, 1, 1),
            EndDate = DateTime.UtcNow.AddMonths(-1)
        };

        // Act & Assert
        experience.IsCurrent.Should().BeFalse();
    }
}