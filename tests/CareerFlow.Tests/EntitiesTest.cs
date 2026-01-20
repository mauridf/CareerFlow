using CareerFlow.Domain.Entities;
using CareerFlow.Domain.Enums;

namespace CareerFlow.Tests;

public class EntitiesTest
{
    [Fact]
    public void User_Entity_Should_Have_Collections_Initialized()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        Assert.NotNull(user.SocialMedias);
        Assert.NotNull(user.Skills);
        Assert.NotNull(user.ProfessionalExperiences);
        Assert.NotNull(user.AcademicBackgrounds);
        Assert.NotNull(user.Certificates);
        Assert.NotNull(user.Languages);
    }

    [Fact]
    public void AcademicBackground_IsCurrent_Should_Return_Correct_Value()
    {
        // Arrange
        var background = new AcademicBackground
        {
            StartDate = new DateTime(2020, 1, 1),
            EndDate = new DateTime(2023, 12, 31)
        };

        // Act & Assert
        Assert.False(background.IsCurrent); // Ended in 2023
    }

    [Fact]
    public void Skill_Default_Values_Should_Be_Set()
    {
        // Arrange & Act
        var skill = new Skill();

        // Assert
        Assert.Equal(SkillType.TOOLS, skill.Type);
        Assert.Equal(SkillLevel.BASIC, skill.Level);
    }
}
