using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Shared.Policies.Dish;

public class DishOwnerRequirement: IAuthorizationRequirement { }

public class DishOwnerHandler(ApplicationDbContext dbContext): AuthorizationHandler<DishOwnerRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, DishOwnerRequirement requirement)
    {
        var user = context.User;
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

        if (context.Resource is not HttpContext resource) return;
        if (user.Identity == null || user.Identity.IsAuthenticated == false) return;

        var parseSuccess = Guid.TryParse(resource.GetRouteValue("dishId")?.ToString(), out var dishId);
        if (!parseSuccess)
        {
            context.Fail(new AuthorizationFailureReason(this, DishError.DishNotFoundCode));
            return;
        }
        
        var dish = await dbContext.Dishes
            .Include(dish => dish.Restaurant)
            .FirstOrDefaultAsync(dish => dish.Id == dishId);

        if (dish == null)
        {
            context.Fail(new AuthorizationFailureReason(this, DishError.DishNotFoundCode));
            return;
        }
        if (dish.Restaurant.HeadId != userId) return;
        
        context.Succeed(requirement);
    }
}