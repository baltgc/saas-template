namespace saas_template.Models.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateUserDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdateUserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public bool? IsActive { get; set; }
}

