using FluentAssertions;
using Moq;
using saas_template.Common.Exceptions;
using saas_template.Models.DTOs;
using saas_template.Models.Entities;
using saas_template.Repositories;
using saas_template.Services;
using saas_template.Tests.Fixtures;

namespace saas_template.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICacheService> _cacheServiceMock;
    private readonly Mock<IRepository<User>> _repositoryMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cacheServiceMock = new Mock<ICacheService>();
        _repositoryMock = new Mock<IRepository<User>>();

        _unitOfWorkMock.Setup(u => u.Repository<User>()).Returns(_repositoryMock.Object);

        _userService = new UserService(_unitOfWorkMock.Object, _cacheServiceMock.Object);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsersFromCache_WhenCacheExists()
    {
        // Arrange
        var cachedUsers = new List<UserDto> { UserFixture.CreateUserDto(1) };
        _cacheServiceMock.Setup(c => c.GetAsync<List<UserDto>>("users_all"))
            .ReturnsAsync(cachedUsers);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().BeEquivalentTo(cachedUsers);
        _repositoryMock.Verify(r => r.GetAllAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllUsersAsync_ShouldReturnUsersFromRepository_WhenCacheMiss()
    {
        // Arrange
        var users = new List<User> { UserFixture.CreateUser(1) };
        _cacheServiceMock.Setup(c => c.GetAsync<List<UserDto>>("users_all"))
            .ReturnsAsync((List<UserDto>?)null);
        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllUsersAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be(users.First().Name);
        _cacheServiceMock.Verify(c => c.SetAsync("users_all", It.IsAny<List<UserDto>>(), It.IsAny<TimeSpan>()), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnUserFromCache_WhenCacheExists()
    {
        // Arrange
        var cachedUser = UserFixture.CreateUserDto(1);
        _cacheServiceMock.Setup(c => c.GetAsync<UserDto>("user_1"))
            .ReturnsAsync(cachedUser);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(cachedUser);
        _repositoryMock.Verify(r => r.GetByIdAsync(1), Times.Never);
    }

    [Fact]
    public async Task GetUserByIdAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        _cacheServiceMock.Setup(c => c.GetAsync<UserDto>("user_1"))
            .ReturnsAsync((UserDto?)null);
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(1);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateUserAsync_ShouldThrowBadRequestException_WhenEmailExists()
    {
        // Arrange
        var createDto = UserFixture.CreateUserDto(email: "existing@example.com");
        var existingUser = UserFixture.CreateUser(email: "existing@example.com");
        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.CreateUserAsync(createDto));
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser_WhenEmailIsUnique()
    {
        // Arrange
        var createDto = UserFixture.CreateUserDto();
        var newUser = UserFixture.CreateUser();
        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync((User?)null);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync(newUser);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.CreateUserAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(createDto.Email);
        result.Name.Should().Be(createDto.Name);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("users_all"), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldReturnNull_WhenUserNotFound()
    {
        // Arrange
        var updateDto = UserFixture.UpdateUserDto(name: "Updated Name");
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.UpdateUserAsync(1, updateDto);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldThrowBadRequestException_WhenEmailExists()
    {
        // Arrange
        var existingUser = UserFixture.CreateUser(1, email: "existing@example.com");
        var anotherUser = UserFixture.CreateUser(2, email: "another@example.com");
        var updateDto = UserFixture.UpdateUserDto(email: "another@example.com");

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingUser);
        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync(anotherUser);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _userService.UpdateUserAsync(1, updateDto));
    }

    [Fact]
    public async Task UpdateUserAsync_ShouldUpdateUser_WhenValid()
    {
        // Arrange
        var user = UserFixture.CreateUser(1);
        var updateDto = UserFixture.UpdateUserDto(name: "Updated Name");
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _repositoryMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>()))
            .ReturnsAsync((User?)null);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.UpdateUserAsync(1, updateDto);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        user.Name.Should().Be("Updated Name");
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("user_1"), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("users_all"), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldReturnFalse_WhenUserNotFound()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((User?)null);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteUserAsync_ShouldDeleteUser_WhenUserExists()
    {
        // Arrange
        var user = UserFixture.CreateUser(1);
        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _userService.DeleteUserAsync(1);

        // Assert
        result.Should().BeTrue();
        _repositoryMock.Verify(r => r.DeleteAsync(user), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("user_1"), Times.Once);
        _cacheServiceMock.Verify(c => c.RemoveAsync("users_all"), Times.Once);
    }
}

