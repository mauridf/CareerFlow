using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class UserTests
{
    [Fact]
    public void Create_ValidData_ShouldCreateUser()
    {
        // Act
        var user = User.Create("João Silva", "joao@email.com", "hashedpassword123");

        // Assert
        user.Id.Should().NotBe(Guid.Empty);
        user.Name.Should().Be("João Silva");
        user.Email.Should().Be("joao@email.com");
        user.Role.Should().Be(UserRole.User);
        user.IsActive.Should().BeFalse(); // Aguarda verificação de email
        user.IsPremium.Should().BeFalse();
        user.DomainEvents.Should().HaveCount(1); // UserRegisteredEvent
    }

    [Theory]
    [InlineData("", "email@teste.com", "hash")]
    [InlineData("Nome", "", "hash")]
    public void Create_InvalidData_ShouldThrowException(string name, string email, string passwordHash)
    {
        // Act
        var action = () => User.Create(name, email, passwordHash);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifyEmail_ShouldSetVerifiedAt()
    {
        // Arrange
        var user = User.Create("Teste", "teste@email.com", "hash");

        // Act
        user.VerifyEmail();

        // Assert
        user.IsEmailVerified().Should().BeTrue();
        user.EmailVerifiedAt.Should().NotBeNull();
        user.IsActive.Should().BeTrue();
    }

    [Fact]
    public void RecordFailedLogin_AfterFiveAttempts_ShouldLockAccount()
    {
        // Arrange
        var user = User.Create("Teste", "teste@email.com", "hash");

        // Act - 5 tentativas falhas
        for (int i = 0; i < 5; i++)
            user.RecordFailedLogin();

        // Assert
        user.FailedLoginAttempts.Should().Be(5);
        user.IsLocked().Should().BeTrue();
        user.CanLogin().Should().BeFalse();
    }

    [Fact]
    public void RecordSuccessfulLogin_ShouldResetFailedAttempts()
    {
        // Arrange
        var user = User.Create("Teste", "teste@email.com", "hash");
        user.RecordFailedLogin();
        user.RecordFailedLogin();

        // Act
        user.RecordSuccessfulLogin();

        // Assert
        user.FailedLoginAttempts.Should().Be(0);
        user.LastLoginAt.Should().NotBeNull();
        user.LockedUntil.Should().BeNull();
    }

    [Fact]
    public void ActivatePremium_ShouldSetPremiumFlag()
    {
        // Arrange
        var user = User.Create("Teste", "teste@email.com", "hash");
        var until = DateTime.UtcNow.AddYears(1);

        // Act
        user.ActivatePremium(until);

        // Assert
        user.IsPremium.Should().BeTrue();
        user.PremiumUntil.Should().Be(until);
        user.Role.Should().Be(UserRole.PremiumUser);
    }

    [Fact]
    public void SoftDelete_ShouldSetDeletedAt()
    {
        // Arrange
        var user = User.Create("Teste", "teste@email.com", "hash");

        // Act
        user.SoftDelete();

        // Assert
        user.DeletedAt.Should().NotBeNull();
        user.IsActive.Should().BeFalse();
        user.CanLogin().Should().BeFalse();
    }
}
