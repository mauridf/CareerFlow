using CareerFlow.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void CreateUser_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var name = "John Doe";
        var email = "john.doe@example.com";
        var passwordHash = "hashed_password";

        // Act
        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash
        };

        // Assert
        user.Should().NotBeNull();
        user.Name.Should().Be(name);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.Id.Should().NotBe(Guid.Empty);
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void User_Collections_ShouldBeInitialized()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.SocialMedias.Should().NotBeNull();
        user.Skills.Should().NotBeNull();
        user.ProfessionalExperiences.Should().NotBeNull();
        user.AcademicBackgrounds.Should().NotBeNull();
        user.Certificates.Should().NotBeNull();
        user.Languages.Should().NotBeNull();
    }
}