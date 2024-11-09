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
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (context.Resource is not HttpContext resource) return;
        if (user.Identity == null || user.Identity.IsAuthenticated == false) return;

        var parseSuccess = Guid.TryParse(resource.GetRouteValue("id")?.ToString(), out var restaurantId);
        if (!parseSuccess)
        {
            var tryGetIdFromBody = await GetRestaurantId(resource);
            if (tryGetIdFromBody.IsFailure)
            {
                context.Fail(new AuthorizationFailureReason(this, RestaurantError.RestaurantNotExistCode));
                return;
            }
            restaurantId = tryGetIdFromBody.Value;
        }

        var restaurantResult = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurantResult == null)
        {
            context.Fail(new AuthorizationFailureReason(this, RestaurantError.RestaurantNotExistCode));
            return;
        }
        if (restaurantResult.HeadId != userId) return;
        
        context.Succeed(requirement);
    }

    private static async Task<Result<Guid>> GetRestaurantId(HttpContext context)
    {
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
            var property = document.RootElement.EnumerateObject()
                .FirstOrDefault(prop => string.Compare(prop.Name, "restaurantId", StringComparison.OrdinalIgnoreCase) == 0);
            if (property.Value.ValueKind != JsonValueKind.Undefined)
            {
                return Result<Guid>.Success(Guid.Parse(property.Value.ToString()));
            }
        }
        catch
        {
            // If any error occurs during body reading, return empty string
        }

        return Result<Guid>.Failure(AppError.MissingRequiredField("Restaurant Id"));
    }
}