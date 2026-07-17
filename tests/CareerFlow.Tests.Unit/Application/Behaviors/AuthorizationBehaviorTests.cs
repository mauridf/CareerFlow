using CareerFlow.Application.Common.Behaviors;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Core.Interfaces;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace CareerFlow.Tests.Unit.Application.Behaviors;

public class AuthorizationBehaviorTests
{
    private readonly Mock<ICurrentUserService> _currentUserMock;

    public AuthorizationBehaviorTests()
    {
        _currentUserMock = new Mock<ICurrentUserService>();
    }

    private async Task<string> ExecuteBehavior<TRequest>(TRequest request, RequestHandlerDelegate<string> next)
        where TRequest : IRequest<string>
    {
        var behaviorType = typeof(AuthorizationBehavior<,>).MakeGenericType(typeof(TRequest), typeof(string));
        var behavior = (IPipelineBehavior<TRequest, string>)Activator.CreateInstance(behaviorType, _currentUserMock.Object)!;
        return await behavior.Handle(request, next, CancellationToken.None);
    }

    private RequestHandlerDelegate<string> SuccessNext() => () => Task.FromResult("success");

    [Fact]
    public async Task NonAuthorizedRequest_ShouldPass()
    {
        var result = await ExecuteBehavior(new NonAuthRequest(), SuccessNext());
        result.Should().Be("success");
    }

    [Fact]
    public async Task AuthorizedRequest_AuthenticatedUser_ShouldPass()
    {
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.Setup(x => x.IsPremium).Returns(false);

        var result = await ExecuteBehavior(new TestRequest(), SuccessNext());
        result.Should().Be("success");
    }

    [Fact]
    public async Task AuthorizedRequest_UnauthenticatedUser_ShouldThrow()
    {
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(false);

        var act = () => ExecuteBehavior(new TestRequest(), SuccessNext());
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task AuthorizedRequest_WrongRole_ShouldThrow()
    {
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.Setup(x => x.Role).Returns("User");

        var act = () => ExecuteBehavior(new AdminRequest(), SuccessNext());
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task AuthorizedRequest_CorrectRole_ShouldPass()
    {
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.Setup(x => x.Role).Returns("Admin");

        var result = await ExecuteBehavior(new AdminRequest(), SuccessNext());
        result.Should().Be("success");
    }

    [Fact]
    public async Task PremiumRequest_NonPremiumUser_ShouldThrow()
    {
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.Setup(x => x.IsPremium).Returns(false);

        var act = () => ExecuteBehavior(new PremiumRequest(), SuccessNext());
        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact]
    public async Task PremiumRequest_PremiumUser_ShouldPass()
    {
        _currentUserMock.Setup(x => x.IsAuthenticated).Returns(true);
        _currentUserMock.Setup(x => x.IsPremium).Returns(true);

        var result = await ExecuteBehavior(new PremiumRequest(), SuccessNext());
        result.Should().Be("success");
    }

    public record TestRequest : IRequest<string>, IAuthorizedRequest
    {
        public string? RequiredRole => null;
        public bool RequirePremium => false;
    }

    public record AdminRequest : IRequest<string>, IAuthorizedRequest
    {
        public string? RequiredRole => "Admin";
        public bool RequirePremium => false;
    }

    public record PremiumRequest : IRequest<string>, IAuthorizedRequest
    {
        public string? RequiredRole => null;
        public bool RequirePremium => true;
    }

    public record NonAuthRequest : IRequest<string>;
}
