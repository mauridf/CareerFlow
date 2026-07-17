using System.Net;
using System.Net.Http.Json;
using CareerFlow.Application.Features.Auth.DTOs;
using FluentAssertions;
using Xunit;

namespace CareerFlow.Tests.Integration.Controllers;

[Collection("Database")]
public class AuthControllerTests : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly DatabaseFixture _fixture;

    public AuthControllerTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        var factory = new CustomWebApplicationFactory(fixture);
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        // Limpa dados
        _fixture.DbContext.Users.RemoveRange(_fixture.DbContext.Users);
        _fixture.DbContext.Persons.RemoveRange(_fixture.DbContext.Persons);
        await _fixture.DbContext.SaveChangesAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_ValidData_ShouldReturnCreated()
    {
        // Arrange
        var request = new RegisterUserRequest("Teste API", "testeapi@test.com", "Senha@123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        ((bool)content!.success).Should().BeTrue();
        ((string)content.data.name).Should().Be("Teste API");
    }

    [Fact]
    public async Task Register_DuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        var request = new RegisterUserRequest("Duplicado", "duplicado@test.com", "Senha@123");
        await _client.PostAsJsonAsync("/api/v1/auth/register", request); // Primeiro registro

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", request); // Duplicado

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Login_ValidCredentials_ShouldReturnToken()
    {
        // Arrange - Registra primeiro
        var registerRequest = new RegisterUserRequest("Login Test", "logintest@test.com", "Senha@123");
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        // Verifica email manualmente (já que não temos envio de email)
        var user = await _fixture.DbContext.Users
            .FirstOrDefaultAsync(u => u.Email == "logintest@test.com");
        user!.VerifyEmail();
        user.Activate();
        await _fixture.DbContext.SaveChangesAsync();

        var loginRequest = new LoginRequest("logintest@test.com", "Senha@123");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        ((string)content!.data.accessToken).Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_InvalidPassword_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("naoexiste@test.com", "WrongPass1");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithoutToken_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetMe_WithValidToken_ShouldReturnUserProfile()
    {
        // Arrange - Registra e loga
        var registerRequest = new RegisterUserRequest("Me Test", "metest@test.com", "Senha@123");
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        var registerContent = await registerResponse.Content.ReadFromJsonAsync<dynamic>();
        var token = (string)registerContent!.data.accessToken;

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/v1/auth/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadFromJsonAsync<dynamic>();
        ((string)content!.data.email).Should().Be("metest@test.com");
    }
}

internal static class Extensions
{
    public static Task<T?> FirstOrDefaultAsync<T>(this Microsoft.EntityFrameworkCore.DbSet<T> dbSet,
        System.Linq.Expressions.Expression<Func<T, bool>> predicate,
        CancellationToken ct = default) where T : class
    {
        return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions
            .FirstOrDefaultAsync(dbSet, predicate, ct);
    }
}
