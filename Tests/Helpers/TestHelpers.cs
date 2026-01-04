using Microsoft.EntityFrameworkCore;
using saas_template.Data;
using saas_template.Models.Entities;

namespace saas_template.Tests.Helpers;

public static class TestHelpers
{
    public static ApplicationDbContext CreateInMemoryDbContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static User CreateTestUser(int id = 1, string? name = null, string? email = null)
    {
        return new User
        {
            Id = id,
            Name = name ?? $"Test User {id}",
            Email = email ?? $"test{id}@example.com",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static List<User> CreateTestUsers(int count = 5)
    {
        return Enumerable.Range(1, count)
            .Select(i => CreateTestUser(i, $"User {i}", $"user{i}@example.com"))
            .ToList();
    }
}

