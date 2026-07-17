using CareerFlow.Core.Entities;
using CareerFlow.Core.Enums;
using CareerFlow.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Integration.Repositories;

[Collection("Database")]
public class UserRepositoryTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly UserRepository _repository;

    public UserRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new UserRepository(fixture.DbContext);
    }

    public async Task InitializeAsync()
    {
        // Limpa dados antes de cada teste
        _fixture.DbContext.Users.RemoveRange(_fixture.DbContext.Users);
        await _fixture.DbContext.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ShouldPersistUser()
    {
        // Arrange
        var user = User.Create("Teste Integration", "integration@test.com", "hashed_password");
        user.VerifyEmail();

        // Act
        var saved = await _repository.AddAsync(user);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        saved.Should().NotBeNull();
        saved.Id.Should().NotBe(Guid.Empty);

        var retrieved = await _repository.GetByEmailAsync("integration@test.com");
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Teste Integration");
        retrieved.Email.Should().Be("integration@test.com");
        retrieved.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnNull_WhenNotFound()
    {
        // Act
        var user = await _repository.GetByEmailAsync("naoexiste@test.com");

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public async Task EmailExistsAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var user = User.Create("Exists", "exists@test.com", "hash");
        await _repository.AddAsync(user);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var exists = await _repository.EmailExistsAsync("exists@test.com");

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task FindPaginatedAsync_ShouldReturnCorrectPage()
    {
        // Arrange - Cria 5 usuários
        for (int i = 1; i <= 5; i++)
        {
            var user = User.Create($"User {i}", $"user{i}@test.com", "hash");
            await _repository.AddAsync(user);
        }
        await _fixture.DbContext.SaveChangesAsync();

        // Act - Página 1, 2 itens
        var (items, total) = await _repository.FindPaginatedAsync(null, 1, 2);

        // Assert
        items.Should().HaveCount(2);
        total.Should().Be(5);
    }
}
