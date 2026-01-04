using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using saas_template.Data;
using saas_template.Models.Entities;
using saas_template.Repositories;
using saas_template.Tests.Helpers;

namespace saas_template.Tests.Repositories;

public class RepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly Repository<User> _repository;

    public RepositoryTests()
    {
        _context = TestHelpers.CreateInMemoryDbContext();
        _repository = new Repository<User>(_context);
    }

    [Fact]
    public async Task AddAsync_ShouldAddEntity()
    {
        // Arrange
        var user = TestHelpers.CreateTestUser();

        // Act
        var result = await _repository.AddAsync(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnEntity_WhenExists()
    {
        // Arrange
        var user = TestHelpers.CreateTestUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(user.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEntities()
    {
        // Arrange
        var users = TestHelpers.CreateTestUsers(3);
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task FirstOrDefaultAsync_ShouldReturnFirstMatchingEntity()
    {
        // Arrange
        var users = TestHelpers.CreateTestUsers(3);
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.FirstOrDefaultAsync(u => u.Email == "user2@example.com");

        // Assert
        result.Should().NotBeNull();
        result!.Email.Should().Be("user2@example.com");
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateEntity()
    {
        // Arrange
        var user = TestHelpers.CreateTestUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        user.Name = "Updated Name";

        // Act
        await _repository.UpdateAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Users.FindAsync(user.Id);
        updated!.Name.Should().Be("Updated Name");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveEntity()
    {
        // Arrange
        var user = TestHelpers.CreateTestUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(user);
        await _context.SaveChangesAsync();

        // Assert
        var deleted = await _context.Users.FindAsync(user.Id);
        deleted.Should().BeNull();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenEntityExists()
    {
        // Arrange
        var user = TestHelpers.CreateTestUser();
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync(u => u.Email == user.Email);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenEntityNotExists()
    {
        // Act
        var result = await _repository.ExistsAsync(u => u.Email == "nonexistent@example.com");

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task CountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var users = TestHelpers.CreateTestUsers(5);
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAsync();

        // Assert
        result.Should().Be(5);
    }

    [Fact]
    public async Task CountAsync_WithPredicate_ShouldReturnFilteredCount()
    {
        // Arrange
        var users = TestHelpers.CreateTestUsers(5);
        _context.Users.AddRange(users);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.CountAsync(u => u.IsActive);

        // Assert
        result.Should().Be(5);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

