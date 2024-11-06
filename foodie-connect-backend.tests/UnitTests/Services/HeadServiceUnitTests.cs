using FluentEmail.Core;
using FluentEmail.Core.Models;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Heads;
using foodie_connect_backend.Modules.Heads.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace food_connect_backend.tests.UnitTests.Services;

public class HeadsServiceUnitTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<IUploaderService> _mockUploaderService;
    private readonly HeadsService _headsService;
    private readonly Mock<IFluentEmail> _mockEmailService;

    public HeadsServiceUnitTests()
    {
        // Setup UserManager mock
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
        _mockEmailService = new Mock<IFluentEmail>();
        _mockUploaderService = new Mock<IUploaderService>();
        var mockVerificationService = new VerificationService(_mockUserManager.Object, _mockEmailService.Object);

        _headsService = new HeadsService(
            _mockUserManager.Object,
            mockVerificationService,
            _mockUploaderService.Object);
    }

    [Fact]
    public async Task CreateHead_Success()
    {
        // Arrange
        var createHeadDto = new CreateHeadDto
        {
            DisplayName = "Test Head",
            PhoneNumber = "1234567890",
            UserName = "testhead",
            Email = "test@example.com",
            Password = "Password123!"
        };

        var createdUser = new User
        {
            Id = "test-id",
            DisplayName = createHeadDto.DisplayName,
            PhoneNumber = createHeadDto.PhoneNumber,
            UserName = createHeadDto.UserName,
            Email = createHeadDto.Email
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), createHeadDto.Password))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(createdUser);

        _mockUserManager.Setup(x => x.AddToRoleAsync(createdUser, "Head"))
            .ReturnsAsync(IdentityResult.Success);

        _mockEmailService.Setup(e => e.SendAsync(default)).ReturnsAsync(new SendResponse() { ErrorMessages = null });

        // Act
        var result = await _headsService.CreateHead(createHeadDto);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(createdUser.Id, result.Value.Id);
        Assert.Equal(createHeadDto.DisplayName, result.Value.DisplayName);
        Assert.Equal(createHeadDto.Email, result.Value.Email);
        
        _mockUserManager.Verify(x => x.CreateAsync(It.IsAny<User>(), createHeadDto.Password), Times.Once);
        _mockUserManager.Verify(x => x.AddToRoleAsync(createdUser, "Head"), Times.Once);
    }

    [Fact]
    public async Task CreateHead_DuplicateUsername_ReturnsConflictError()
    {
        // Arrange
        var createHeadDto = new CreateHeadDto
        {
            DisplayName = "Test Head",
            UserName = "existinguser",
            Email = "test@example.com",
            Password = "Password123!"
        };

        _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), createHeadDto.Password))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "DuplicateUserName", Description = "Username already exists" }));

        // Act
        var result = await _headsService.CreateHead(createHeadDto);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Conflict", result.Error.Code);
        Assert.Equal("Username already exists", result.Error.Message);
    }

    [Fact]
    public async Task GetHeadById_Success()
    {
        // Arrange
        var userId = "test-id";
        var user = new User { Id = userId, UserName = "testhead" };

        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync(user);
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(true);

        // Act
        var result = await _headsService.GetHeadById(userId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userId, result.Value.Id);
        
        _mockUserManager.Verify(x => x.FindByIdAsync(userId), Times.Once);
        _mockUserManager.Verify(x => x.IsInRoleAsync(user, "Head"), Times.Once);
    }

    [Fact]
    public async Task GetHeadById_UserNotFound_ReturnsError()
    {
        // Arrange
        var userId = "nonexistent-id";
        _mockUserManager.Setup(x => x.FindByIdAsync(userId))
            .ReturnsAsync((User)null!);

        // Act
        var result = await _headsService.GetHeadById(userId);

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
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _headsService.ChangePassword(userId, changePasswordDto);

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
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Code = "PasswordMismatch", Description = "Incorrect password" }));

        // Act
        var result = await _headsService.ChangePassword(userId, changePasswordDto);

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
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(true);
        _mockUserManager.Setup(x => x.UpdateAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        _mockUploaderService.Setup(x => x.UploadImageAsync(
                It.IsAny<IFormFile>(),
                It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Success(avatarId));

        // Act
        var result = await _headsService.UploadAvatar(userId, mockFile.Object);

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
        _mockUserManager.Setup(x => x.IsInRoleAsync(user, "Head"))
            .ReturnsAsync(true);

        _mockUploaderService.Setup(x => x.UploadImageAsync(
                It.IsAny<IFormFile>(),
                It.IsAny<ImageFileOptions>()))
            .ReturnsAsync(Result<string>.Failure(uploadError));

        // Act
        var result = await _headsService.UploadAvatar(userId, mockFile.Object);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(uploadError.Code, result.Error.Code);
        Assert.Equal(uploadError.Message, result.Error.Message);
    }
}