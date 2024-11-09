using System.Text;
using System.Text.Json;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Policies.Dish;
using foodie_connect_backend.Shared.Policies.Restaurant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace foodie_connect_backend.Shared.Policies;

public class AuthorizationResponseTransformer : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (context.User?.Identity?.IsAuthenticated == false)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsJsonAsync(AuthError.NotAuthenticated());
            return;
        }
        
        if (!authorizeResult.Succeeded)
        {
            var failureReason = authorizeResult.AuthorizationFailure?.FailureReasons
                .FirstOrDefault()?.Message;

            var response = new AuthorizationResponse 
                { StatusCode = StatusCodes.Status403Forbidden, Error = AuthError.NotAuthorized() };

            if (failureReason is not null)
            {
                response = failureReason switch
                {
                    RestaurantError.RestaurantNotExistCode => new AuthorizationResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = RestaurantError.RestaurantNotExist()
                    },
                    DishError.DishNotFoundCode => new AuthorizationResponse
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = DishError.DishNotFound()
                    },
                    _ => new AuthorizationResponse
                    {
                        StatusCode = StatusCodes.Status403Forbidden, 
                        Error = AuthError.NotAuthorized()
                    }
                };
            }

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response.Error);
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}

internal record AuthorizationResponse
{
    public int StatusCode { get; init; }
    public AppError Error { get; init; } = null!;
}