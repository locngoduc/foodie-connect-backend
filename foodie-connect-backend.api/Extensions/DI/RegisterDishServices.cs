using foodie_connect_backend.Modules.DishCategories;
using foodie_connect_backend.Modules.Dishes;
using foodie_connect_backend.Modules.Dishes.Hub;
using foodie_connect_backend.Modules.DishReviews;

namespace foodie_connect_backend.Extensions.DI;

public static class RegisterDishServices
{
    public static void AddDishServices(this IServiceCollection services)
    {
        services.AddSingleton<ActiveDishViewersService>();

        services.AddScoped<DishesService>();
        services.AddScoped<DishReviewsService>();
        services.AddScoped<DishCategoriesService>();
    }
}