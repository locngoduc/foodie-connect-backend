using FluentEmail.Core;
using FluentEmail.Core.Models;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Dtos;
using foodie_connect_backend.Uploader;
using foodie_connect_backend.Users;
using foodie_connect_backend.Users.Dtos;
using foodie_connect_backend.Verification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace food_connect_backend.tests.UnitTests.Services;

public class UsersServiceUnitTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<IUploaderService> _mockUploaderService;
    private readonly Mock<SignInManager<User>> _mockSignInManager;
    private readonly UsersService _usersService;
    private readonly Mock<IFluentEmail> _mockEmailService;

    public UsersServiceUnitTests()
    {
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
            
        _mockEmailService = new Mock<IFluentEmail>();
        _mockUploaderService = new Mock<IUploaderService>();
        var mockVerificationService = new VerificationService(_mockUserManager.Object, _mockEmailService.Object);
        
        var contextAccessor = new Mock<IHttpContextAccessor>();
        var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
        _mockSignInManager = new Mock<SignInManager<User>>(
            _mockUserManager.Object,
            contextAccessor.Object,
            claimsFactory.Object,
            null!, null!, null!, null!);

        _usersService = new UsersService(
            _mockUserManager.Object,
            mockVerificationService,
            _mockUploaderService.Object,
            _mockSignInManager.Object);
    }

    [Fact]
    public async Task CreateUser_Success()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            DisplayName = "Test User",
            PhoneNumber = "1234567890",
            UserName = "testuser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), createUserDto.Password))
            .ReturnsAsync(IdentityResult.Success);
            
        _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "User"))
            .ReturnsAsync(IdentityResult.Success);
            
        _mockEmailService.Setup(e => e.SendAsync(default))
            .ReturnsAsync(new SendResponse { ErrorMessages = null });

        // Act
        var result = await _usersService.CreateUser(createUserDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(createUserDto.DisplayName, result.Value.DisplayName);
        Assert.Equal(createUserDto.Email, result.Value.Email);
        
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), createUserDto.Password), Times.Once);
        _mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<User>(), "User"), Times.Once);
    }

    [Fact]
    public async Task CreateUser_DuplicateUsername_ReturnsConflictError()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            DisplayName = "Test User",
            UserName = "existinguser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), createUserDto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = "Username already exists" }));

        // Act
        var result = await _usersService.CreateUser(createUserDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Conflict", result.Error.Code);
        Assert.Equal("Username already exists", result.Error.Message);
    }

    [Fact]
    public async Task GetUserById_Success()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId, UserName = "testuser" };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);

        // Act
        var result = await _usersService.GetUserById(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.Id);
        
        _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        _mockUserManager.Verify(x => x.IsInRoleAsync(user, "User"), Times.Once);
    }

    [Fact]
    public async Task GetUserById_UserNotFound_ReturnsError()
    {
        // Arrange
        var userId = "nonexistent-id";
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _usersService.GetUserById(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("RecordNotFound", result.Error.Code);
    }

    [Fact]
    public async Task ChangePassword_Success()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };
        var changePasswordDto = new ChangePasswordDto
        {
            OldPassword = "OldPass123!",
            NewPassword = "NewPass123!"
        };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _usersService.ChangePassword(userId, changePasswordDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        
        _mockUserManager.Verify(x => x.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_WrongPassword_ReturnsError()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };
        var changePasswordDto = new ChangePasswordDto
        {
            OldPassword = "WrongPass123!",
            NewPassword = "NewPass123!"
        };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PasswordMismatch", Description = "Incorrect password" }));

        // Act
        var result = await _usersService.ChangePassword(userId, changePasswordDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InvalidCredential", result.Error.Code);
    }

    [Fact]
    public async Task UploadAvatar_Success()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };
        var mockFile = new Mock<IFormFile>();
        var avatarId = "avatar-123";

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        _mockUploaderService.Setup(x => x.UploadImageAsync(
                It.IsAny<IFormFile>(),
                It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Success(avatarId));

        // Act
        var result = await _usersService.UploadAvatar(userId, mockFile.Object);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        Assert.Equal(avatarId, user.AvatarId);
        
        _mockUploaderService.Verify(x => x.UploadImageAsync(
            It.IsAny<IFormFile>(),
            It.Is<ImageFileOptions>(o => 
                o.Format == "webp" && 
                o.PublicId == userId && 
                o.Folder == "foodie/user_avatars")), 
            Times.Once);
    }

    [Fact]
    public async Task UploadAvatar_UploadFails_ReturnsError()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };
        var mockFile = new Mock<IFormFile>();
        var uploadError = AppError.ValidationError("Upload failed");

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);

        _mockUploaderService.Setup(x => x.UploadImageAsync(
                It.IsAny<IFormFile>(),
                It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Failure(uploadError));

        // Act
        var result = await _usersService.UploadAvatar(userId, mockFile.Object);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(uploadError.Code, result.Error.Code);
        Assert.Equal(uploadError.Message, result.Error.Message);
    }

    [Fact]
    public async Task UpgradeToHead_Success()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.AddToRoleAsync(user, "Head"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _usersService.UpgradeToHead(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value);
        
        _mockUserManager.Verify(x => x.RemoveFromRoleAsync(user, "User"), Times.Once);
        _mockUserManager.Verify(x => x.AddToRoleAsync(user, "Head"), Times.Once);
        _mockSignInManager.Verify(x => x.SignOutAsync(), Times.Once);
    }

    [Fact]
    public async Task UpgradeToHead_FailedToRemoveUserRole_ReturnsError()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to remove role" }));

        // Act
        var result = await _usersService.UpgradeToHead(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InternalError", result.Error.Code);
        Assert.Equal("Failed to remove User role", result.Error.Message);
    }

    [Fact]
    public async Task UpgradeToHead_FailedToAddHeadRole_RollsBackAndReturnsError()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "User"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.RemoveFromRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);
        _mockUserManager.Setup(x => x.AddToRoleAsync(user, "Head"))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Failed to add role" }));
        _mockUserManager.Setup(x => x.AddToRoleAsync(user, "User"))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _usersService.UpgradeToHead(userId);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("InternalError", result.Error.Code);
        Assert.Equal("Failed to add Head role", result.Error.Message);
        
        _mockUserManager.Verify(x => x.AddToRoleAsync(user, "User"), Times.Once);
    }
}