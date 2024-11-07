using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Policies.Restaurant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace foodie_connect_backend.Shared.Policies;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded)
        {
            var failureReason = authorizeResult.AuthorizationFailure?.FailureReasons
                .FirstOrDefault()?.Message;

            var response = failureReason switch
            {
                RestaurantAuthorizationFailureReason.RestaurantNotFound => 
                    new { 
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = RestaurantError.RestaurantNotExist(
                            context.GetRouteValue("id")?.ToString() ?? string.Empty)
                    },
                RestaurantAuthorizationFailureReason.NotOwner => 
                    new { 
                        StatusCode = StatusCodes.Status403Forbidden,
                        Error = AuthError.NotAuthorized()
                    },
                _ => new { 
                    StatusCode = StatusCodes.Status403Forbidden,
                    Error = AuthError.NotAuthorized()
                }
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response.Error);
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}