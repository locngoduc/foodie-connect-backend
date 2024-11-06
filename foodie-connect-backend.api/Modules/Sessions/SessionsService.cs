using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Sessions.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Modules.Sessions;

public class SessionsService(
    SignInManager<User> signInManager,
    UserManager<User> userManager)
{
    public async Task<Result<bool>> LoginHead(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null || await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(AppError.InvalidCredential("Invalid username or password"));

        var result = await signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Result<bool>.Failure(AppError.InvalidCredential("Invalid username or password"));

        return Result<bool>.Success(true);
    }
    
    public async Task<Result<bool>> LoginUser(string username, string password)
    {
        var user = await userManager.FindByNameAsync(username);
        if (user == null || await userManager.IsInRoleAsync(user, "Head"))
            return Result<bool>.Failure(AppError.InvalidCredential("Invalid username or password"));

        var result = await signInManager.PasswordSignInAsync(username, password, isPersistent: true, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Result<bool>.Failure(AppError.InvalidCredential("Invalid username or password"));

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> Logout()
    {
        await signInManager.SignOutAsync();
        return Result<bool>.Success(true);
    }

    public async Task<Result<SessionInfo>> GetUserSession(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result<SessionInfo>.Failure(AppError.InvalidCredential("Invalid username or password"));
        var roles = await userManager.GetRolesAsync(user);
        var sessionResponse = new SessionInfo
        {
            Type = roles.FirstOrDefault()!,
            Id = user.Id,
            UserName = user.UserName!,
            DisplayName = user.DisplayName,
            Avatar = user.AvatarId,
            Email = user.Email!,
            EmailConfirmed = user.EmailConfirmed,
            PhoneNumber = user.PhoneNumber!,
        };
        return Result<SessionInfo>.Success(sessionResponse);
    }
}