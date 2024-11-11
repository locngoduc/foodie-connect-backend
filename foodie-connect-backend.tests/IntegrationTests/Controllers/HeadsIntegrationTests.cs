using System.Net;
using System.Text;
using Castle.Core.Configuration;
using foodie_connect_backend;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Users.Dtos;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace food_connect_backend.tests.IntegrationTests.Controllers;

public class UsersIntegrationTests(FoodieWebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task CreateUser_ValidData_ReturnsCreatedUser()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            UserName = "test_CreateUser",
            Email = "test_CreateUser@example.com",
            Password = "Password123!",
            DisplayName = "user_CreateUser",
            PhoneNumber = "0123456789",
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(createDto),
            Encoding.UTF8,
            "application/json");

        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.PostAsync("/v1/users", content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var user = JsonConvert.DeserializeObject<UserResponseDto>(responseString);
        Assert.NotNull(user);
        Assert.Equal(createDto.UserName, user.UserName);
        Assert.Equal(createDto.DisplayName, user.DisplayName);
        
        // Verify Location header
        Assert.NotNull(response.Headers.Location);
        Assert.Contains($"/v1/users/{user.Id}", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task CreateUser_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var email = "duplicateuseremail@exampletest.com";
        var createDto1 = new CreateUserDto
        {
            UserName = "test_user_1",
            Email = email,
            Password = "Password123!",
            DisplayName = $"user_test_1",
            PhoneNumber = "0123456788"
        };

        var createDto2 = new CreateUserDto
        {
            UserName = "test_user_2",
            Email = email,
            Password = "Password123!",
            DisplayName = $"user_test_2",
            PhoneNumber = "0123456789"
        };

        var client = CreateUnauthenticatedClient();

        // Create first user
        await client.PostAsync("/v1/users", new StringContent(
            JsonConvert.SerializeObject(createDto1),
            Encoding.UTF8,
            "application/json"));

        // Act - Try to create second user with same email
        var response = await client.PostAsync("/v1/users", new StringContent(
            JsonConvert.SerializeObject(createDto2),
            Encoding.UTF8,
            "application/json"));

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetUser_ExistingId_ReturnsUser()
    {
        // Arrange - Create a user first
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        var userId = authenticatedClient.User.Id;

        // Act
        var response = await authenticatedClient.Client.GetAsync($"/v1/users/{userId}");
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var user = JsonConvert.DeserializeObject<UserResponseDto>(responseString);
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
        Assert.Equal(authenticatedClient.User.UserName, user.UserName);
    }

    [Fact]
    public async Task GetUser_NonexistentId_ReturnsNotFound()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync($"/v1/users/{Guid.NewGuid()}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UploadAvatar_ValidFile_ReturnsSuccess()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        
        // Create test image file
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        
        var formData = new MultipartFormDataContent();
        formData.Add(imageContent, "file", "test-avatar.png");

        // Act
        var response = await authenticatedClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/avatar", 
            formData);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        Assert.NotNull(result);
        Assert.Equal("Avatar updated successfully", result.Message);
    }

    [Fact]
    public async Task UploadAvatar_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        var differentClient = await CreateAuthenticatedClientAsync("User");
        
        var imageContent = new ByteArrayContent([0x42]);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
        
        var formData = new MultipartFormDataContent();
        formData.Add(imageContent, "file", "test-avatar.png");

        // Act - Try to upload avatar for different user
        var response = await differentClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/avatar", 
            formData);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_ValidData_ReturnsSuccess()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        
        var changePasswordDto = new ChangePasswordDto
        {
            OldPassword = "Password123!", // Default password from CreateAuthenticatedClientAsync
            NewPassword = "NewPassword123!"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(changePasswordDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await authenticatedClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/password", 
            content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        Assert.NotNull(result);
        Assert.Equal("Password changed successfully", result.Message);
    }

    [Fact]
    public async Task ChangePassword_WrongOldPassword_ReturnsUnauthorized()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        
        var changePasswordDto = new ChangePasswordDto
        {
            OldPassword = "WrongPassword123!",
            NewPassword = "NewPassword123!"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(changePasswordDto),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await authenticatedClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/password", 
            content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        var differentClient = await CreateAuthenticatedClientAsync("User");
        
        var changePasswordDto = new ChangePasswordDto
        {
            OldPassword = "Password123!",
            NewPassword = "NewPassword123!"
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(changePasswordDto),
            Encoding.UTF8,
            "application/json");

        // Act - Try to change password for different user
        var response = await differentClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/password", 
            content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpgradeToHead_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");

        // Act
        var response = await authenticatedClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/type",
            new StringContent(""));
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        Assert.NotNull(result);
        Assert.Equal("Upgraded user to head successfully", result.Message);
    }

    [Fact]
    public async Task UpgradeToHead_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
        var differentClient = await CreateAuthenticatedClientAsync("User");

        // Act - Try to upgrade different user
        var response = await differentClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/type",
            new StringContent(""));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpgradeToHead_AlreadyHead_ReturnsForbidden()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("Head");

        // Act - Try to upgrade when already a head
        var response = await authenticatedClient.Client.PatchAsync(
            $"/v1/users/{authenticatedClient.User.Id}/type",
            new StringContent(""));

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}