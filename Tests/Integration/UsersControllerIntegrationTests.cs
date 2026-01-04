using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using saas_template.Data;
using saas_template.Models.DTOs;
using saas_template.Models.Entities;
using saas_template.Tests.Fixtures;
using System.Net;
using System.Net.Http.Json;

namespace saas_template.Tests.Integration;

public class UsersControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public UsersControllerIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real DbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                });

                // Remove cache service to test without Redis
                var cacheDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(saas_template.Services.ICacheService));
                if (cacheDescriptor != null)
                {
                    services.Remove(cacheDescriptor);
                }

                // Add mock cache service
                services.AddScoped<saas_template.Services.ICacheService, MockCacheService>();
            });
        });

        _client = _factory.CreateClient();
        var scope = _factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnEmptyList_WhenNoUsers()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<IEnumerable<UserDto>>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedUser()
    {
        // Arrange
        var createDto = UserFixture.CreateUserDto();

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/users", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(createDto.Email);
        result.Data.Name.Should().Be(createDto.Name);
    }

    [Fact]
    public async Task GetUser_ShouldReturnUser_WhenExists()
    {
        // Arrange
        var user = UserFixture.CreateUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/api/v1/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task UpdateUser_ShouldUpdateUser_WhenExists()
    {
        // Arrange
        var user = UserFixture.CreateUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var updateDto = UserFixture.UpdateUserDto(name: "Updated Name");

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/users/{user.Id}", updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        result.Should().NotBeNull();
        result!.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteUser_ShouldDeleteUser_WhenExists()
    {
        // Arrange
        var user = UserFixture.CreateUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/users/{user.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var deleted = await _context.Users.FindAsync(user.Id);
        deleted.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
        _client.Dispose();
    }
}

// Mock cache service for integration tests
public class MockCacheService : saas_template.Services.ICacheService
{
    private readonly Dictionary<string, object> _cache = new();

    public Task<T?> GetAsync<T>(string key) where T : class
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult(value as T);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
    {
        _cache[key] = value!;
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.ContainsKey(key));
    }
}

