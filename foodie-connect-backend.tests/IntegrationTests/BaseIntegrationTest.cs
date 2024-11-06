using System.Net.Http.Json;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Sessions.Dtos;
using foodie_connect_backend.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace food_connect_backend.tests.IntegrationTests;

public class BaseIntegrationTest: IClassFixture<FoodieWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly FoodieWebApplicationFactory<Program> _factory;
    protected readonly ApplicationDbContext Context;
    protected readonly UserManager<User> UserManager;
    private readonly IConfiguration _configuration;
    
    protected BaseIntegrationTest(FoodieWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        var scope = factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        UserManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        _configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
    }

    protected HttpClient CreateUnauthenticatedClient()
    {
        return _factory.CreateClient();
    }
    
    protected async Task<FoodieClientWrapper> CreateAuthenticatedClientAsync(string userType = "User")
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("origin", _configuration["FRONTEND_URL"]);
        
        var user = new User
        {
            UserName = $"test_{Guid.NewGuid()}@example.com",
            Email = $"test_{Guid.NewGuid()}@example.com",
            PhoneNumber = "0123456789",
            DisplayName = $"user_{Guid.NewGuid()}",
            EmailConfirmed = true
        };

        var result = await UserManager.CreateAsync(user, "Password123!");
        if (!result.Succeeded)
        {
            throw new Exception($"Could not create user: {string.Join(", ", result.Errors)}");
        }

        await UserManager.AddToRoleAsync(user, userType);

        var loginResponse = await client.PostAsJsonAsync("/v1/session", new LoginDto
        {
            UserName = user.UserName,
            Password = "Password123!",
            LoginType = userType == "User" ? LoginType.User : LoginType.Head
        });

        if (!loginResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Could not authenticate user. Status code: {loginResponse.StatusCode}");
        }

        return new FoodieClientWrapper(client, user);
    }
}