using System.Text;
using System.Text.Json;
using foodie_connect_backend.Shared.Classes;
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

            var result = await GetRestaurantId(context);
            if (result.IsFailure)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(AppError.UnexpectedError("Guid is not valid"));
                return;
            }
            
            var response = failureReason switch
            {
                RestaurantAuthorizationFailureReason.RestaurantNotFound => 
                    new { 
                        StatusCode = StatusCodes.Status404NotFound,
                        Error = RestaurantError.RestaurantNotExist(result.Value)
                    },
                RestaurantAuthorizationFailureReason.NotOwner => 
                    new { 
                        StatusCode = StatusCodes.Status403Forbidden,
                        Error = RestaurantError.NotOwner(result.Value)
                    },
                SessionFailureReason.NotAuthenticated =>
                    new
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        Error = AuthError.NotAuthenticated()
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