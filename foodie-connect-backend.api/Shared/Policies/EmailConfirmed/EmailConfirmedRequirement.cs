using foodie_connect_backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Shared.Policies.EmailConfirmed;

public class EmailConfirmedRequirement : IAuthorizationRequirement { }

public class EmailConfirmedHandler(UserManager<User> userManager)
    : AuthorizationHandler<EmailConfirmedRequirement>
{

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, 
        EmailConfirmedRequirement requirement)
    {
        var user = context.User;
        if (user.Identity == null || user.Identity.IsAuthenticated == false) return;

        var dbUser = await userManager.GetUserAsync(context.User);
        if (dbUser is { EmailConfirmed: true }) context.Succeed(requirement);
    }
}