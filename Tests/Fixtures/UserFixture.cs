using saas_template.Models.DTOs;
using saas_template.Models.Entities;

namespace saas_template.Tests.Fixtures;

public class UserFixture
{
    public static CreateUserDto CreateUserDto(string? name = null, string? email = null)
    {
        return new CreateUserDto
        {
            Name = name ?? "Test User",
            Email = email ?? "test@example.com"
        };
    }

    public static UpdateUserDto UpdateUserDto(string? name = null, string? email = null, bool? isActive = null)
    {
        return new UpdateUserDto
        {
            Name = name,
            Email = email,
            IsActive = isActive
        };
    }

    public static User CreateUser(int id = 1, string? name = null, string? email = null, bool isActive = true)
    {
        return new User
        {
            Id = id,
            Name = name ?? $"User {id}",
            Email = email ?? $"user{id}@example.com",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static UserDto CreateUserDto(int id = 1, string? name = null, string? email = null, bool isActive = true)
    {
        return new UserDto
        {
            Id = id,
            Name = name ?? $"User {id}",
            Email = email ?? $"user{id}@example.com",
            IsActive = isActive,
            CreatedAt = DateTime.UtcNow
        };
    }
}

