using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Sessions;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace food_connect_backend.tests.UnitTests.Services;

public class SessionsServiceTests
{
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly SessionsService _sessionsService;

    public SessionsServiceTests()
    {
        var mockUserStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            mockUserStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);

        var mockContextAccessor = new Mock<IHttpContextAccessor>();
        var mockClaimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            mockContextAccessor.Object,
            mockClaimsFactory.Object,
            null!, null!, null!, null!);

        _sessionsService = new SessionsService(_mockSignInManager.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task LoginHead_WithValidHeadCredentials_ReturnsSuccess()
    {
        // Arrange
        var username = "head@example.com";
        var password = "Password123!";
        var user = new User { UserName = username };

        _mockUserManager.Setup(x => x.FindByNameAsync(username))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(false);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, password, true, false))
            .ReturnsAsync(SignInResult.Success);

        // Act
        var result = await _sessionsService.LoginHead(username, password);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
    
    [Fact]
    public async Task LoginHead_WithInvalidHeadCredentials_ReturnsFailure()
    {
        // Arrange
        var username = "head@example.com";
        var password = "Password123!";
        var wrongPassword = "Wrong password!";
        var user = new User { UserName = username };

        _mockUserManager.Setup(x => x.FindByNameAsync(username))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(false);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, password, true, false))
            .ReturnsAsync(SignInResult.Success);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, wrongPassword, true, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _sessionsService.LoginHead(username, wrongPassword);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task LoginHead_WithUserRole_ReturnsFailure()
    {
        // Arrange
        var username = "user@example.com";
        var password = "Password123!";
        var user = new User { UserName = username };

        _mockUserManager.Setup(x => x.FindByNameAsync(username))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);

        // Act
        var result = await _sessionsService.LoginHead(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(AuthError.InvalidCredentialsCode, result.Error.Code);
    }

    [Fact]
    public async Task LoginUser_WithValidUserCredentials_ReturnsSuccess()
    {
        // Arrange
        var username = "user@example.com";
        var password = "Password123!";
        var user = new User { UserName = username };

        _mockUserManager.Setup(x => x.FindByNameAsync(username))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(false);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, password, true, false))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        // Act
        var result = await _sessionsService.LoginUser(username, password);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
    }
    
    [Fact]
    public async Task LoginUser_WithInvalidHeadCredentials_ReturnsFailure()
    {
        // Arrange
        var username = "head@example.com";
        var password = "Password123!";
        var wrongPassword = "Wrong password!";
        var user = new User { UserName = username };

        _mockUserManager.Setup(x => x.FindByNameAsync(username))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(false);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, password, true, false))
            .ReturnsAsync(SignInResult.Success);
        _mockSignInManager.Setup(x => x.PasswordSignInAsync(username, wrongPassword, true, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _sessionsService.LoginUser(username, wrongPassword);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task LoginUser_WithHeadRole_ReturnsFailure()
    {
        // Arrange
        var username = "head@example.com";
        var password = "Password123!";
        var user = new User { UserName = username };

        _mockUserManager.Setup(x => x.FindByNameAsync(username))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(true);

        // Act
        var result = await _sessionsService.LoginUser(username, password);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(AuthError.InvalidCredentialsCode, result.Error.Code);
    }

    [Fact]
    public async Task Logout_Always_ReturnsSuccess()
    {
        // Arrange
        _mockSignInManager.Setup(x => x.SignOutAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _sessionsService.Logout();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        _mockSignInManager.Verify(x => x.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task GetUserSession_WithValidUserId_ReturnsSessionInfo()
    {
        // Arrange
        var userId = "123";
        var user = new User
        {
            Id = userId,
            UserName = "test@example.com",
            DisplayName = "Test User",
            Email = "test@example.com",
            EmailConfirmed = true,
            PhoneNumber = "1234567890",
            AvatarId = "avatar123"
        };
        var roles = new List<string> { "User" };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(roles);

        // Act
        var result = await _sessionsService.GetUserSession(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.Id);
        Assert.Equal(user.UserName, result.Value.UserName);
        Assert.Equal(user.DisplayName, result.Value.DisplayName);
        Assert.Equal(user.Email, result.Value.Email);
        Assert.Equal(user.EmailConfirmed, result.Value.EmailConfirmed);
        Assert.Equal(user.PhoneNumber, result.Value.PhoneNumber);
        Assert.Equal(user.AvatarId, result.Value.Avatar);
        Assert.Equal(roles[0], result.Value.Type);
    }

    [Fact]
    public async Task GetUserSession_WithInvalidUserId_ReturnsFailure()
    {
        // Arrange
        var userId = "invalid";
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sessionsService.GetUserSession(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(AuthError.InvalidCredentialsCode, result.Error.Code);
    }
}