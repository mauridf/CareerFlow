using CareerFlow.Core.Entities;
using CareerFlow.Core.Events;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class CertificateTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Create_ValidData_ShouldCreateCertificate()
    {
        var cert = Certificate.Create(
            _personId, "AWS Certified", "Amazon",
            new DateTime(2024, 1, 1));

        cert.Title.Should().Be("AWS Certified");
        cert.Issuer.Should().Be("Amazon");
        cert.IsActive.Should().BeTrue();
        cert.DomainEvents.Should().HaveCount(1);
        cert.DomainEvents.Should().AllBeOfType<CertificateCreatedEvent>();
    }

    [Fact]
    public void Create_EmptyTitle_ShouldThrow()
    {
        var action = () => Certificate.Create(
            _personId, "", "Amazon", DateTime.Now);

        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Update_ShouldRaiseUpdatedEvent()
    {
        var cert = Certificate.Create(
            _personId, "AWS Certified", "Amazon",
            new DateTime(2024, 1, 1));

        cert.Update("Azure Certified", "Microsoft",
            new DateTime(2024, 6, 1), null);

        cert.Title.Should().Be("Azure Certified");
        cert.DomainEvents.Should().HaveCount(2);
        cert.DomainEvents.Should().ContainItemsAssignableTo<CertificateUpdatedEvent>();
    }
}
