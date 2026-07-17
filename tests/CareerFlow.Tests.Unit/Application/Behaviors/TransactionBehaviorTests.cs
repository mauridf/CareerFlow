using CareerFlow.Application.Common.Behaviors;
using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Core.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CareerFlow.Tests.Unit.Application.Behaviors;

public class TransactionBehaviorTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TransactionBehavior<TestTransactionalRequest, string> _behavior;
    private readonly TransactionBehavior<TestNonTransactionalRequest, string> _nonTransactionalBehavior;

    public TransactionBehaviorTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        var loggerMock = new Mock<ILogger<TransactionBehavior<TestTransactionalRequest, string>>>();
        _behavior = new TransactionBehavior<TestTransactionalRequest, string>(
            _unitOfWorkMock.Object, loggerMock.Object);
        _nonTransactionalBehavior = new TransactionBehavior<TestNonTransactionalRequest, string>(
            _unitOfWorkMock.Object, Mock.Of<ILogger<TransactionBehavior<TestNonTransactionalRequest, string>>>());
    }

    [Fact]
    public async Task NonTransactionalRequest_ShouldNotBeginTransaction()
    {
        var request = new TestNonTransactionalRequest();

        var result = await _nonTransactionalBehavior.Handle(
            request, () => Task.FromResult("success"), CancellationToken.None);

        result.Should().Be("success");
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TransactionalRequest_ShouldBeginAndCommitTransaction()
    {
        var request = new TestTransactionalRequest();

        var result = await _behavior.Handle(
            request, () => Task.FromResult("success"), CancellationToken.None);

        result.Should().Be("success");
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TransactionalRequest_Failure_ShouldRollback()
    {
        var request = new TestTransactionalRequest();

        var act = () => _behavior.Handle(
            request,
            () => throw new InvalidOperationException("fail"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _unitOfWorkMock.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.RollbackTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(x => x.CommitTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    public record TestTransactionalRequest : IRequest<string>, ITransactionalRequest;

    public record TestNonTransactionalRequest : IRequest<string>;
}
