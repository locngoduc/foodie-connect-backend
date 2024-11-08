using System.Security.Claims;
using System.Text;
using System.Text.Json;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace foodie_connect_backend.Shared.Policies.Restaurant;

public class RestaurantOwnerRequirement : IAuthorizationRequirement { }

public class RestaurantOwnerHandler(ApplicationDbContext dbContext) : AuthorizationHandler<RestaurantOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        RestaurantOwnerRequirement requirement)
    {
        var user = context.User;

        if (context.Resource is not HttpContext resource) return;
        if (user.Identity == null || user.Identity.IsAuthenticated == false)
        {
            context.Fail(new AuthorizationFailureReason(this, SessionFailureReason.NotAuthenticated));
            return;
        }

        var restaurantId = resource.GetRouteValue("id") as Guid?;

        // If not in route, try to get from body
        if (restaurantId == null)
        {
            var result = await GetRestaurantId(resource);
            if (result.IsSuccess) restaurantId = result.Value;
        }

        if (restaurantId == null)
        {
            context.Fail(new AuthorizationFailureReason(this, RestaurantAuthorizationFailureReason.RestaurantNotFound));
            return;
        }

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

    private static async Task<Result<Guid>> GetRestaurantId(HttpContext context)
    {
        // First try to get from route
        var routeId = context.GetRouteValue("id")?.ToString();
        if (!string.IsNullOrEmpty(routeId))
        {
            return Result<Guid>.Success(Guid.Parse(routeId));
        }

        // If not in route, try to get from body
        try
        {
            context.Request.EnableBuffering();
            
            using var reader = new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: -1,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("restaurantId", out var idElement))
            {
                return Result<Guid>.Success(Guid.Parse(idElement.GetString()!));
            }
        }
        catch
        {
            // If any error occurs during body reading, return empty string
        }

        return Result<Guid>.Failure(AppError.None);
    }
}