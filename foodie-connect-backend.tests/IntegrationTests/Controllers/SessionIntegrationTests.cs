using System.Net;
using System.Text;
using foodie_connect_backend.Data;
using foodie_connect_backend.Sessions.Dtos;
using foodie_connect_backend.Shared.Dtos;
using foodie_connect_backend.Shared.Enums;
using Newtonsoft.Json;

namespace food_connect_backend.tests.IntegrationTests.Controllers;

public class SessionsIntegrationTests(FoodieWebApplicationFactory<Program> factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Login_ValidUserCredentials_ReturnsSuccess()
    {
        // Arrange
        // Create the user
        var user = new User
        {
            UserName = $"test_{Guid.NewGuid()}@example.com",
            Email = $"test_{Guid.NewGuid()}@example.com",
            PhoneNumber = "0123456789",
            DisplayName = $"user_{Guid.NewGuid()}",
            EmailConfirmed = true
        };
        await UserManager.CreateAsync(user, "Password123!");
        await UserManager.AddToRoleAsync(user, "User");
        
        var loginDto = new LoginDto
        {
            UserName = user.UserName,
            Password = "Password123!",
            LoginType = LoginType.User
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(loginDto),
            Encoding.UTF8,
            "application/json");

        var client = CreateUnauthenticatedClient();
    
        // Act
        var response = await client.PostAsync("/v1/session", content);
        var responseString = await response.Content.ReadAsStringAsync();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        Assert.NotNull(result);
        Assert.Equal("Successfully logged in", result.Message);
    }

    [Fact]
    public async Task Login_ValidHeadCredentials_ReturnsSuccess()
    {
        // Arrange
        // Create the head
        var user = new User
        {
            UserName = $"test_{Guid.NewGuid()}@example.com",
            Email = $"test_{Guid.NewGuid()}@example.com",
            PhoneNumber = "0123456789",
            DisplayName = $"user_{Guid.NewGuid()}",
            EmailConfirmed = true
        };
        await UserManager.CreateAsync(user, "Password123!");
        await UserManager.AddToRoleAsync(user, "Head");
        
        var loginDto = new LoginDto
        {
            UserName = user.UserName,
            Password = "Password123!",
            LoginType = LoginType.Head
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(loginDto),
            Encoding.UTF8,
            "application/json");

        var client = CreateUnauthenticatedClient();
    
        // Act
        var response = await client.PostAsync("/v1/session", content);
        var responseString = await response.Content.ReadAsStringAsync();
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = JsonConvert.DeserializeObject<GenericResponse>(responseString);
        Assert.NotNull(result);
        Assert.Equal("Successfully logged in", result.Message);
    }
    
    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UserName = "wronguser",
            Password = "wrongpassword",
            LoginType = LoginType.User
        };
    
        var content = new StringContent(
            JsonConvert.SerializeObject(loginDto),
            Encoding.UTF8,
            "application/json");
    
        var client = CreateUnauthenticatedClient();
    
        // Act
        var response = await client.PostAsync("/v1/session", content);
    
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Login_InvalidLoginType_ReturnsBadRequest()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            UserName = "testuser",
            Password = "testpassword",
            LoginType = (LoginType)99 // Invalid login type
        };
    
        var content = new StringContent(
            JsonConvert.SerializeObject(loginDto),
            Encoding.UTF8,
            "application/json");
    
        var client = CreateUnauthenticatedClient();
    
        // Act
        var response = await client.PostAsync("/v1/session", content);
    
        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task GetSessionInfo_AuthenticatedUser_ReturnsSessionInfo()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
    
        // Act
        var response = await authenticatedClient.Client.GetAsync("/v1/session");
        var responseString = await response.Content.ReadAsStringAsync();
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var sessionInfo = JsonConvert.DeserializeObject<SessionInfo>(responseString);
        Assert.NotNull(sessionInfo);
        Assert.Equal(authenticatedClient.User.Id, sessionInfo.Id);
    }
    
    [Fact]
    public async Task GetSessionInfo_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
    
        // Act
        var response = await client.GetAsync("/v1/session");
    
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
    
    [Fact]
    public async Task Logout_AuthenticatedUser_ReturnsSuccess()
    {
        // Arrange
        var authenticatedClient = await CreateAuthenticatedClientAsync("User");
    
        // Act
        var logoutRequest = await authenticatedClient.Client.DeleteAsync("/v1/session");
        var logoutResponseString = await logoutRequest.Content.ReadAsStringAsync();
        var logoutResponse = JsonConvert.DeserializeObject<GenericResponse>(logoutResponseString);

        var sessionRequest = await authenticatedClient.Client.GetAsync("/v1/session");
    
        // Assert
        Assert.Equal(HttpStatusCode.OK, logoutRequest.StatusCode);
        Assert.NotNull(logoutResponse);
        Assert.Equal("Successfully logged out", logoutResponse.Message);
        Assert.Equal(HttpStatusCode.Unauthorized, sessionRequest.StatusCode);
    }
    
    [Fact]
    public async Task Logout_UnauthenticatedUser_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();
    
        // Act
        var response = await client.DeleteAsync("/v1/session");
    
        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}