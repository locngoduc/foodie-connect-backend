using System.Net.Http.Json;
using foodie_connect_backend.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace food_connect_backend.tests.IntegrationTests;

public class BaseIntegrationTest: IClassFixture<FoodieWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly FoodieWebApplicationFactory<Program> _factory;
    protected readonly ApplicationDbContext Context;
    private readonly UserManager<User> _userManager;
    protected BaseIntegrationTest(FoodieWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
        var scope = factory.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    }
    
    protected async Task<FoodieClientWrapper> CreateAuthenticatedClientAsync(string userType = "User")
    {
        var client = _factory.CreateClient();
        var user = new User
        {
            UserName = $"test_{Guid.NewGuid()}@example.com",
            Email = $"test_{Guid.NewGuid()}@example.com",
            PhoneNumber = "0123456789",
            DisplayName = $"user_{Guid.NewGuid()}",
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, "Password123!");
        if (!result.Succeeded)
        {
            throw new Exception($"Could not create user: {string.Join(", ", result.Errors)}");
        }

        await _userManager.AddToRoleAsync(user, userType);

        var registrationResponse = await client.PostAsJsonAsync("/v1/session", new
        {
            UserName = user.UserName,
            Password = "Password123!"
        });

        if (!registrationResponse.IsSuccessStatusCode)
        {
            throw new Exception($"Could not authenticate user. Status code: {registrationResponse.StatusCode}");
        }

        var cookies = registrationResponse.Headers.GetValues("Set-Cookie");
        client.DefaultRequestHeaders.Add("Cookie", cookies);

        return new FoodieClientWrapper(client, user);
    }
}