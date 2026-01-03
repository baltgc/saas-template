using saas_template.Common.Exceptions;
using saas_template.Models.DTOs;
using saas_template.Models.Entities;
using saas_template.Repositories;

namespace saas_template.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;

    public UserService(IRepository<User> userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        return MapToDto(user);
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if email already exists
        var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
        if (existingUser != null)
            throw new BadRequestException("User with this email already exists");

        var user = new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            IsActive = true
        };

        var createdUser = await _userRepository.AddAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        // Check if email is being changed and if it already exists
        if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
        {
            var existingUser = await _userRepository.FirstOrDefaultAsync(u => u.Email == updateUserDto.Email);
            if (existingUser != null)
                throw new BadRequestException("User with this email already exists");
        }

        if (!string.IsNullOrEmpty(updateUserDto.Name))
            user.Name = updateUserDto.Name;

        if (!string.IsNullOrEmpty(updateUserDto.Email))
            user.Email = updateUserDto.Email;

        if (updateUserDto.IsActive.HasValue)
            user.IsActive = updateUserDto.IsActive.Value;

        user.UpdatedAt = DateTime.UtcNow;

        await _userRepository.UpdateAsync(user);
        return MapToDto(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        await _userRepository.DeleteAsync(user);
        return true;
    }

    private static UserDto MapToDto(User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
    }
}

