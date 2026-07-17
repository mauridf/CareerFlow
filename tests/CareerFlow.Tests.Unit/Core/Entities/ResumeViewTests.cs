using System.Net;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Events;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Unit.Core.Entities;

public class ResumeViewTests
{
    private readonly Guid _personId = Guid.NewGuid();

    [Fact]
    public void Record_ShouldRaiseViewedEvent()
    {
        var ip = IPAddress.Parse("192.168.1.1");
        var view = ResumeView.Record(_personId, ip, "Mozilla/5.0", null, "direct");

        view.PersonId.Should().Be(_personId);
        view.IpAddress.Should().Be(ip);
        view.Source.Should().Be("direct");
        view.PdfDownloaded.Should().BeFalse();
        view.DomainEvents.Should().HaveCount(1);
        view.DomainEvents.Should().AllBeOfType<ResumeViewedEvent>();
    }

    [Fact]
    public void MarkPdfDownloaded_ShouldSetFlag()
    {
        var view = ResumeView.Record(_personId, null, null);

        view.MarkPdfDownloaded();

        view.PdfDownloaded.Should().BeTrue();
    }
}
