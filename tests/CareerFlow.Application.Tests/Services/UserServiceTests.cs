using AutoMapper;
using CareerFlow.Application.Common;
using CareerFlow.Application.DTOs;
using CareerFlow.Application.Interfaces;
using CareerFlow.Application.Services;
using CareerFlow.Domain.Common;
using CareerFlow.Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CareerFlow.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IApplicationDbContext> _mockContext;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly Mock<IJwtTokenGenerator> _mockJwtTokenGenerator;
    private readonly Mock<IOptions<JwtSettings>> _mockJwtSettings;
    private readonly Mock<IFileStorageService> _mockFileStorageService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockContext = new Mock<IApplicationDbContext>();
        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<UserService>>();
        _mockJwtTokenGenerator = new Mock<IJwtTokenGenerator>();
        _mockJwtSettings = new Mock<IOptions<JwtSettings>>();
        _mockFileStorageService = new Mock<IFileStorageService>();

        var jwtSettings = new JwtSettings { ExpiryInMinutes = 60 };
        _mockJwtSettings.Setup(x => x.Value).Returns(jwtSettings);

        _userService = new UserService(
            _mockContext.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockJwtTokenGenerator.Object,
            _mockJwtSettings.Object,
            _mockFileStorageService.Object);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Name = "John Doe",
            Email = "john.doe@example.com",
            Password = "Password@123",
            City = "São Paulo",
            State = "SP"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            PasswordHash = "hashed_password"
        };

        var userDto = new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        var mockSet = new Mock<DbSet<User>>();
        _mockContext.Setup(c => c.Users).Returns(mockSet.Object);
        _mockContext.Setup(c => c.Users.Add(It.IsAny<User>())).Callback<User>(u => user = u);
        _mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _mockMapper.Setup(m => m.Map<User>(createUserDto)).Returns(user);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _userService.CreateAsync(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createUserDto.Name);
        result.Email.Should().Be(createUserDto.Email);
        _mockContext.Verify(c => c.Users.Add(It.IsAny<User>()), Times.Once);
        _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var userDto = new UserDto
        {
            Id = userId,
            Name = "John Doe",
            Email = "john.doe@example.com"
        };

        var mockSet = new Mock<DbSet<User>>();
        mockSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(user);
        _mockContext.Setup(c => c.Users).Returns(mockSet.Object);
        _mockMapper.Setup(m => m.Map<UserDto>(user)).Returns(userDto);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(userId);
        result.Name.Should().Be(user.Name);
    }
}