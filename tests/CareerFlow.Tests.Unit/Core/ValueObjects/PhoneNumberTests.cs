using CareerFlow.Core.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("(11) 98765-4321", "11", "11987654321")]
    [InlineData("11987654321", "11", "11987654321")]
    [InlineData("(21) 3456-7890", "21", "2134567890")]
    public void Create_ValidPhone_ShouldFormatCorrectly(string input, string expectedDdd, string expectedDigits)
    {
        // Act
        var phone = new PhoneNumber(input);

        // Assert
        phone.DDD.Should().Be(expectedDdd);
        phone.DigitsOnly.Should().Be(expectedDigits);
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("00987654321")]
    public void Create_InvalidPhone_ShouldThrowException(string invalidPhone)
    {
        // Act
        var action = () => new PhoneNumber(invalidPhone);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Equals_SameDigits_ShouldBeEqual()
    {
        // Arrange
        var phone1 = new PhoneNumber("(11) 98765-4321");
        var phone2 = new PhoneNumber("11987654321");

        // Assert
        phone1.Should().Be(phone2);
    }
}
