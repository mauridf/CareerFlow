using CareerFlow.API.Controllers;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using Xunit;

namespace CareerFlow.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IValidator<CreateUserDto>> _mockCreateUserValidator;
    private readonly Mock<IValidator<LoginDto>> _mockLoginValidator;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockCreateUserValidator = new Mock<IValidator<CreateUserDto>>();
        _mockLoginValidator = new Mock<IValidator<LoginDto>>();

        _authController = new AuthController(
            _mockUserService.Object,
            _mockCreateUserValidator.Object,
            _mockLoginValidator.Object);
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "Password@123"
        };

        var userDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockCreateUserValidator.Setup(v => v.ValidateAsync(createUserDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _mockUserService.Setup(s => s.CreateAsync(createUserDto))
            .ReturnsAsync(userDto);

        // Act
        var result = await _authController.Register(createUserDto);

        // Assert
        var createdResult = result.Result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.Value.Should().BeEquivalentTo(userDto);
        createdResult.ActionName.Should().Be(nameof(AuthController.Register));
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOk()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "john.doe@example.com",
            Password = "Password@123"
        };

        var authResponse = new AuthResponseDto
        {
            Token = "jwt_token",
            User = new UserDto { Id = Guid.NewGuid(), Name = "John Doe", Email = "john.doe@example.com" },
            ExpiresAt = DateTime.UtcNow.AddHours(1)
        };

        var validationResult = new FluentValidation.Results.ValidationResult();
        _mockLoginValidator.Setup(v => v.ValidateAsync(loginDto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _mockUserService.Setup(s => s.AuthenticateAsync(loginDto))
            .ReturnsAsync(authResponse);

        // Act
        var result = await _authController.Login(loginDto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().BeEquivalentTo(authResponse);
    }
}