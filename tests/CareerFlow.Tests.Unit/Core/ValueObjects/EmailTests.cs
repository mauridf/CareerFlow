using CareerFlow.Core.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("joao@email.com")]
    [InlineData("maria.silva@empresa.com.br")]
    [InlineData("user+tag@domain.co")]
    public void Create_ValidEmail_ShouldCreateEmail(string validEmail)
    {
        // Act
        var email = new Email(validEmail);

        // Assert
        email.Value.Should().Be(validEmail);
        email.Domain.Should().NotBeEmpty();
        email.LocalPart.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("invalid")]
    [InlineData("@nodomain.com")]
    [InlineData("noat.com")]
    public void Create_InvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Act
        var action = () => new Email(invalidEmail);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_SameEmail_ShouldBeEqual()
    {
        // Arrange
        var email1 = new Email("teste@email.com");
        var email2 = new Email("teste@email.com");

        // Assert
        email1.Should().Be(email2);
        (email1 == email2).Should().BeTrue();
    }

    [Fact]
    public void NormalizedValue_ShouldBeLowercase()
    {
        // Act
        var email = new Email("Joao.Silva@Email.COM");

        // Assert
        email.NormalizedValue.Should().Be("joao.silva@email.com");
    }
}
