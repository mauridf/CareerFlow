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

public class RegisterUserHandlerTests
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPersonRepository> _personRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<RegisterUserHandler>> _loggerMock;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _personRepoMock = new Mock<IPersonRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _tokenServiceMock = new Mock<ITokenService>();
        _emailServiceMock = new Mock<IEmailService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _loggerMock = new Mock<ILogger<RegisterUserHandler>>();

        _handler = new RegisterUserHandler(
            _userRepoMock.Object,
            _personRepoMock.Object,
            _passwordHasherMock.Object,
            _tokenServiceMock.Object,
            _emailServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldRegisterUser()
    {
        // Arrange
        var command = new RegisterUserCommand("João", "joao@email.com", "Senha@123");

        _userRepoMock.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _passwordHasherMock.Setup(p => p.HashPassword(command.Password))
            .Returns("hashed_password");
        _tokenServiceMock.Setup(t => t.GenerateTokensAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokenResult
            {
                AccessToken = "access_token",
                RefreshToken = "refresh_token",
                AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(15),
                RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(7)
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("João");
        result.Email.Should().Be("joao@email.com");
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");

        _userRepoMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
        _personRepoMock.Verify(r => r.AddAsync(It.IsAny<Person>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingEmail_ShouldThrowConflictException()
    {
        // Arrange
        var command = new RegisterUserCommand("João", "existe@email.com", "Senha@123");

        _userRepoMock.Setup(r => r.EmailExistsAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var action = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await action.Should().ThrowAsync<CareerFlow.Core.Exceptions.ConflictException>();
    }
}
