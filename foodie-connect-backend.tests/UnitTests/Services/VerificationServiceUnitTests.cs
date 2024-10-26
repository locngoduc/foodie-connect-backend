using FluentEmail.Core;
using FluentEmail.Core.Models;
using foodie_connect_backend.Data;
using foodie_connect_backend.Verification;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace food_connect_backend.tests.UnitTests.Services;

public class VerificationServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly Mock<IFluentEmail> _mockFluentEmail;
    private readonly VerificationService _service;

    public VerificationServiceTests()
    {
        _mockUserManager = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
        _mockFluentEmail = new Mock<IFluentEmail>();
        _service = new VerificationService(_mockUserManager.Object, _mockFluentEmail.Object);
    }

    [Fact]
    public async Task SendConfirmationEmail_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = "nonexistentUser";
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _service.SendConfirmationEmail(userId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("User not found", result.Error.Message);
    }

    [Fact]
    public async Task SendConfirmationEmail_EmailAlreadyConfirmed_ReturnsFailure()
    {
        // Arrange
        var userId = "user1";
        var user = new User { Id = userId, EmailConfirmed = true };
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _service.SendConfirmationEmail(userId);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Email is already confirmed", result.Error.Message);
    }

    [Fact]
    public async Task SendConfirmationEmail_SendsEmail_ReturnsSuccess()
    {
        // Arrange
        var userId = "user1";
        var user = new User { Id = userId, Email = "user@example.com", EmailConfirmed = false };
        var token = "email_token";
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.GenerateEmailConfirmationTokenAsync(user)).ReturnsAsync(token);
        _mockFluentEmail.Setup(f => f.To(user.Email))
            .Returns(_mockFluentEmail.Object);
        _mockFluentEmail.Setup(f => f.SetFrom(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_mockFluentEmail.Object);
        _mockFluentEmail.Setup(f => f.Subject(It.IsAny<string>()))
            .Returns(_mockFluentEmail.Object);
        _mockFluentEmail.Setup(f => f.Body(It.IsAny<string>(), default))
            .Returns(_mockFluentEmail.Object);
        _mockFluentEmail.Setup(f => f.SendAsync(default))
            .ReturnsAsync(new SendResponse { ErrorMessages = null });

        // Act
        var result = await _service.SendConfirmationEmail(userId);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserManager.Verify(m => m.GenerateEmailConfirmationTokenAsync(user), Times.Once);
        _mockFluentEmail.Verify(f => f.SendAsync(default), Times.Once);
    }

    [Fact]
    public async Task ConfirmEmail_UserNotFound_ReturnsFailure()
    {
        // Arrange
        var userId = "nonexistentUser";
        var token = "test_token";
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _service.ConfirmEmail(userId, token);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("No user found", result.Error.Message);
    }

    [Fact]
    public async Task ConfirmEmail_InvalidToken_ReturnsFailure()
    {
        // Arrange
        var userId = "user1";
        var token = "invalid_token";
        var user = new User { Id = userId, Email = "user@example.com" };
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Invalid token" }));

        // Act
        var result = await _service.ConfirmEmail(userId, token);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("Token is not valid", result.Error.Message);
    }

    [Fact]
    public async Task ConfirmEmail_ValidToken_ReturnsSuccess()
    {
        // Arrange
        var userId = "user1";
        var token = "valid_token";
        var user = new User { Id = userId, Email = "user@example.com" };
        _mockUserManager.Setup(m => m.FindByIdAsync(userId)).ReturnsAsync(user);
        _mockUserManager.Setup(m => m.ConfirmEmailAsync(user, token)).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.ConfirmEmail(userId, token);

        // Assert
        Assert.True(result.IsSuccess);
        _mockUserManager.Verify(m => m.ConfirmEmailAsync(user, token), Times.Once);
    }
}