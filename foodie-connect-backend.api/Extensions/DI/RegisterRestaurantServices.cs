using foodie_connect_backend.Modules.Promotions;
using foodie_connect_backend.Modules.RestaurantReviews;
using foodie_connect_backend.Modules.Restaurants;
using foodie_connect_backend.Modules.RestaurantServices;
using foodie_connect_backend.Modules.Socials;

namespace foodie_connect_backend.Extensions.DI;

public static class RegisterRestaurantServices
{
    public static void AddRestaurantServices(this IServiceCollection services)
    {
        services.AddScoped<RestaurantsService>();
        services.AddScoped<SocialsService>();
        services.AddScoped<PromotionsService>();
        services.AddScoped<RestaurantReviewsService>();
        services.AddScoped<RestaurantServicesService>();
    }
}