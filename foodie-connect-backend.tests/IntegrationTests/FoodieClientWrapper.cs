using foodie_connect_backend.Data;

namespace food_connect_backend.tests.IntegrationTests;

public class FoodieClientWrapper
{
    public readonly HttpClient Client;
    public readonly User User;

    public FoodieClientWrapper(HttpClient client, User user)
    {
        Client = client;
        User = user;
    }
}