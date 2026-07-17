using CareerFlow.Core.Entities;
using CareerFlow.Infrastructure.Repositories;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Integration.Repositories;

[Collection("Database")]
public class PersonRepositoryTests : IAsyncLifetime
{
    private readonly DatabaseFixture _fixture;
    private readonly PersonRepository _repository;
    private readonly UserRepository _userRepository;

    public PersonRepositoryTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _repository = new PersonRepository(fixture.DbContext);
        _userRepository = new UserRepository(fixture.DbContext);
    }

    public async Task InitializeAsync()
    {
        _fixture.DbContext.Persons.RemoveRange(_fixture.DbContext.Persons);
        _fixture.DbContext.Users.RemoveRange(_fixture.DbContext.Users);
        await _fixture.DbContext.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task AddAsync_ShouldPersistPerson()
    {
        // Arrange
        var user = User.Create("Person User", "person@test.com", "hash");
        await _userRepository.AddAsync(user);
        await _fixture.DbContext.SaveChangesAsync();

        var person = Person.Create(user.Id);
        person.UpdatePersonalInfo("(11) 99999-9999", "São Paulo", "SP", null, null);

        // Act
        await _repository.AddAsync(person);
        await _fixture.DbContext.SaveChangesAsync();

        // Assert
        var retrieved = await _repository.GetByUserIdAsync(user.Id);
        retrieved.Should().NotBeNull();
        retrieved!.City.Should().Be("São Paulo");
        retrieved.State.Should().Be("SP");
    }

    [Fact]
    public async Task GetBySlugAsync_ShouldReturnPerson_WhenPublic()
    {
        // Arrange
        var user = User.Create("Slug User", "slug@test.com", "hash");
        await _userRepository.AddAsync(user);
        await _fixture.DbContext.SaveChangesAsync();

        var person = Person.Create(user.Id);
        person.SetResumeSlug("slug-teste");
        person.SetPublic(true);
        await _repository.AddAsync(person);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var retrieved = await _repository.GetBySlugAsync("slug-teste");

        // Assert
        retrieved.Should().NotBeNull();
        retrieved!.ResumeSlug.Should().Be("slug-teste");
    }

    [Fact]
    public async Task SlugExistsAsync_ShouldReturnTrue_WhenExists()
    {
        // Arrange
        var user = User.Create("Slug2", "slug2@test.com", "hash");
        await _userRepository.AddAsync(user);
        await _fixture.DbContext.SaveChangesAsync();

        var person = Person.Create(user.Id);
        person.SetResumeSlug("meu-slug");
        await _repository.AddAsync(person);
        await _fixture.DbContext.SaveChangesAsync();

        // Act
        var exists = await _repository.SlugExistsAsync("meu-slug");

        // Assert
        exists.Should().BeTrue();
    }
}
