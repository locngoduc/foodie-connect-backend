using System.Security.Claims;
using foodie_connect_backend.Data;
using Microsoft.AspNetCore.Authorization;

namespace foodie_connect_backend.Shared.Policies.Restaurant;

public class RestaurantOwnerRequirement : IAuthorizationRequirement { }

public class RestaurantOwnerHandler(ApplicationDbContext dbContext) : AuthorizationHandler<RestaurantOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        RestaurantOwnerRequirement requirement)
    {
        var user = context.User;
        var resource = context.Resource as HttpContext;
        if (resource == null) return;

        // Extract restaurantId from route
        var restaurantId = resource.GetRouteValue("id")?.ToString();
        if (string.IsNullOrEmpty(restaurantId)) return;

        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId)) return;

        var restaurantResult = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurantResult == null)
        {
            context.Fail(new AuthorizationFailureReason(this, RestaurantAuthorizationFailureReason.RestaurantNotFound));
            return;
        }

        if (restaurantResult.HeadId != userId)
        {
            context.Fail(new AuthorizationFailureReason(this, RestaurantAuthorizationFailureReason.NotOwner));
            return;
        }
        
        context.Succeed(requirement);
    }
}