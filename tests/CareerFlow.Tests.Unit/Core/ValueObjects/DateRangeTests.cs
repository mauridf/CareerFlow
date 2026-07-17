using CareerFlow.Core.ValueObjects;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.ValueObjects;

public class DateRangeTests
{
    [Fact]
    public void Create_ValidRange_ShouldCalculateDuration()
    {
        // Arrange
        var start = new DateTime(2020, 1, 1);
        var end = new DateTime(2024, 12, 31);

        // Act
        var range = new DateRange(start, end);

        // Assert
        range.DurationInMonths.Should().Be(59);
        range.DurationFormatted.Should().Be("4 anos e 11 meses");
        range.IsOngoing.Should().BeFalse();
    }

    [Fact]
    public void Create_NoEndDate_ShouldBeOngoing()
    {
        // Act
        var range = new DateRange(DateTime.Now.AddYears(-2));

        // Assert
        range.IsOngoing.Should().BeTrue();
        range.EndDate.Should().BeNull();
    }

    [Fact]
    public void Create_EndBeforeStart_ShouldThrowException()
    {
        // Act
        var action = () => new DateRange(
            new DateTime(2024, 1, 1),
            new DateTime(2020, 1, 1));

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Contains_DateWithinRange_ShouldReturnTrue()
    {
        // Arrange
        var range = new DateRange(
            new DateTime(2020, 1, 1),
            new DateTime(2024, 12, 31));

        // Act & Assert
        range.Contains(new DateTime(2022, 6, 15)).Should().BeTrue();
        range.Contains(new DateTime(2019, 12, 31)).Should().BeFalse();
        range.Contains(new DateTime(2025, 1, 1)).Should().BeFalse();
    }
}
