using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using saas_template.Common.Helpers;
using saas_template.Models.DTOs;
using saas_template.Services;
using saas_template.Tests.Fixtures;

namespace saas_template.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<ILogger<saas_template.Controllers.UsersController>> _loggerMock;
    private readonly saas_template.Controllers.UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _loggerMock = new Mock<ILogger<saas_template.Controllers.UsersController>>();
        _controller = new saas_template.Controllers.UsersController(_userServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOk_WithUsers()
    {
        // Arrange
        var users = new List<UserDto> { UserFixture.CreateUserDto(1), UserFixture.CreateUserDto(2) };
        _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAllUsers();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<UserDto>>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUser_ShouldReturnOk_WhenUserExists()
    {
        // Arrange
        var user = UserFixture.CreateUserDto(1);
        _userServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetUser(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<UserDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(user);
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserNotExists()
    {
        // Arrange
        _userServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.GetUser(1);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<UserDto>>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreated_WhenUserCreated()
    {
        // Arrange
        var createDto = UserFixture.CreateUserDto();
        var createdUser = UserFixture.CreateUserDto(1);
        _userServiceMock.Setup(s => s.CreateUserAsync(createDto)).ReturnsAsync(createdUser);

        // Act
        var result = await _controller.CreateUser(createDto);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        var response = createdResult.Value.Should().BeOfType<ApiResponse<UserDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(createdUser);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnOk_WhenUserUpdated()
    {
        // Arrange
        var updateDto = UserFixture.UpdateUserDto(name: "Updated Name");
        var updatedUser = UserFixture.CreateUserDto(1, name: "Updated Name");
        _userServiceMock.Setup(s => s.UpdateUserAsync(1, updateDto)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(1, updateDto);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<UserDto>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().BeEquivalentTo(updatedUser);
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnNotFound_WhenUserNotExists()
    {
        // Arrange
        var updateDto = UserFixture.UpdateUserDto();
        _userServiceMock.Setup(s => s.UpdateUserAsync(1, updateDto)).ReturnsAsync((UserDto?)null);

        // Act
        var result = await _controller.UpdateUser(1, updateDto);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<UserDto>>().Subject;
        response.Success.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnOk_WhenUserDeleted()
    {
        // Arrange
        _userServiceMock.Setup(s => s.DeleteUserAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var response = okResult.Value.Should().BeOfType<ApiResponse<bool>>().Subject;
        response.Success.Should().BeTrue();
        response.Data.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNotFound_WhenUserNotExists()
    {
        // Arrange
        _userServiceMock.Setup(s => s.DeleteUserAsync(1)).ReturnsAsync(false);

        // Act
        var result = await _controller.DeleteUser(1);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var response = notFoundResult.Value.Should().BeOfType<ApiResponse<bool>>().Subject;
        response.Success.Should().BeFalse();
    }
}

