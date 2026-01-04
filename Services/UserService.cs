using saas_template.Common.Exceptions;
using saas_template.Models.DTOs;
using saas_template.Models.Entities;
using saas_template.Repositories;

namespace saas_template.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private const string CacheKeyPrefix = "user_";
    private const string CacheKeyAllUsers = "users_all";

    public UserService(IUnitOfWork unitOfWork, ICacheService cacheService)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
    }

    private IRepository<User> UserRepository => _unitOfWork.Repository<User>();

    private static string GetCacheKey(int id) => $"{CacheKeyPrefix}{id}";

    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        // Check cache first
        var cachedUsers = await _cacheService.GetAsync<List<UserDto>>(CacheKeyAllUsers);
        if (cachedUsers != null)
            return cachedUsers;

        var users = await UserRepository.GetAllAsync();
        var userDtos = users.Select(MapToDto).ToList();

        // Cache for 5 minutes
        await _cacheService.SetAsync(CacheKeyAllUsers, userDtos, TimeSpan.FromMinutes(5));

        return userDtos;
    }

    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var cacheKey = GetCacheKey(id);

        // Check cache first
        var cachedUser = await _cacheService.GetAsync<UserDto>(cacheKey);
        if (cachedUser != null)
            return cachedUser;

        var user = await UserRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        var userDto = MapToDto(user);

        // Cache for 5 minutes
        await _cacheService.SetAsync(cacheKey, userDto, TimeSpan.FromMinutes(5));

        return userDto;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        // Check if email already exists
        var existingUser = await UserRepository.FirstOrDefaultAsync(u => u.Email == createUserDto.Email);
        if (existingUser != null)
            throw new BadRequestException("User with this email already exists");

        var user = new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            IsActive = true
        };

        var createdUser = await UserRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = MapToDto(createdUser);

        // Invalidate cache
        await _cacheService.RemoveAsync(CacheKeyAllUsers);

        return userDto;
    }

    public async Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto updateUserDto)
    {
        var user = await UserRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        // Check if email is being changed and if it already exists
        if (!string.IsNullOrEmpty(updateUserDto.Email) && updateUserDto.Email != user.Email)
        {
            var existingUser = await UserRepository.FirstOrDefaultAsync(u => u.Email == updateUserDto.Email);
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

        await UserRepository.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var userDto = MapToDto(user);

        // Invalidate cache
        await _cacheService.RemoveAsync(GetCacheKey(id));
        await _cacheService.RemoveAsync(CacheKeyAllUsers);

        return userDto;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await UserRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        await UserRepository.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();

        // Invalidate cache
        await _cacheService.RemoveAsync(GetCacheKey(id));
        await _cacheService.RemoveAsync(CacheKeyAllUsers);

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

