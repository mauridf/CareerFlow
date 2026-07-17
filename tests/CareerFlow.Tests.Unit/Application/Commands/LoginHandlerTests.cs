using CareerFlow.Application.Common.Interfaces;
using CareerFlow.Application.Features.Auth.Commands;
using CareerFlow.Application.Features.Auth.Handlers;
using CareerFlow.Core.Entities;
using CareerFlow.Core.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CareerFlow.Tests.Unit.Application.Commands;

public class LoginHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<LoginHandler>> _loggerMock;
    private readonly LoginHandler _handler;

    public LoginHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<LoginHandler>>();

        _handler = new LoginHandler(
            _userRepoMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ShouldLogin()
    {
        // Arrange
        var user = User.Create("João", "joao@email.com", "hashed");
        user.VerifyEmail(); // Ativa a conta
        var command = new LoginCommand("joao@email.com", "Senha@123");

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateTokensAsync(user, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenResult { AccessToken = "token", RefreshToken = "refresh" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.Id);
        result.AccessToken.Should().Be("token");
    }

    [Fact]
    public async Task Handle_InvalidPassword_ShouldThrowUnauthorized()
    {
        // Arrange
        var user = User.Create("João", "joao@email.com", "hashed");
        user.VerifyEmail();
        var command = new LoginCommand("joao@email.com", "WrongPassword");

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock.Setup(p => p.VerifyPassword(command.Password, user.PasswordHash))
            .Returns(false);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<CareerFlow.Core.Exceptions.UnauthorizedException>();
    }

    [Fact]
    public async Task Handle_UserNotFound_ShouldThrowUnauthorized()
    {
        // Arrange
        var command = new LoginCommand("naoexiste@email.com", "Senha@123");

        _userRepoMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<CareerFlow.Core.Exceptions.UnauthorizedException>();
    }
}
