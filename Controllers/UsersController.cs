using Microsoft.AspNetCore.Mvc;
using saas_template.Common.Helpers;
using saas_template.Models.DTOs;
using saas_template.Services;

namespace saas_template.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponse<IEnumerable<UserDto>>.ErrorResponse("An error occurred while retrieving users"));
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetUser(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse($"User with ID {id} not found"));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving the user"));
        }
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            var user = await _userService.CreateUserAsync(createUserDto);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, 
                ApiResponse<UserDto>.SuccessResponse(user, "User created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while creating the user"));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateUser(int id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            var user = await _userService.UpdateUserAsync(id, updateUserDto);
            if (user == null)
                return NotFound(ApiResponse<UserDto>.ErrorResponse($"User with ID {id} not found"));

            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while updating the user"));
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(int id)
    {
        try
        {
            var deleted = await _userService.DeleteUserAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<bool>.ErrorResponse($"User with ID {id} not found"));

            return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while deleting the user"));
        }
    }
}

