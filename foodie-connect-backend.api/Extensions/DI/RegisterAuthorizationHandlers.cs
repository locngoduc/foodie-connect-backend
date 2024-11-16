using foodie_connect_backend.Shared.Policies;
using foodie_connect_backend.Shared.Policies.Dish;
using foodie_connect_backend.Shared.Policies.EmailConfirmed;
using foodie_connect_backend.Shared.Policies.Restaurant;
using Microsoft.AspNetCore.Authorization;

namespace foodie_connect_backend.Extensions.DI;

public static class RegisterAuthorizationHandlers
{
    public static void AddAuthorizationHandlers(this IServiceCollection services)
    {
        services.AddScoped<IAuthorizationHandler, RestaurantOwnerHandler>();
        services.AddScoped<IAuthorizationHandler, DishOwnerHandler>();
        services.AddScoped<IAuthorizationHandler, EmailConfirmedHandler>();
        services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResponseTransformer>();

        services.AddAuthorizationBuilder()
            .AddPolicy("RestaurantOwner", policy =>
                policy.Requirements.Add(new RestaurantOwnerRequirement()))
            .AddPolicy("DishOwner", policy =>
                policy.Requirements.Add(new DishOwnerRequirement()))
            .AddPolicy("EmailVerified", policy =>
                policy.Requirements.Add(new EmailConfirmedRequirement()));
    }
}