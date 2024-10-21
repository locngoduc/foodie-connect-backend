using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Verification;

public class VerificationService(UserManager<User> userManager)
{
    public async Task<Result<bool>> ConfirmEmail(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result<bool>.Failure(AppError.RecordNotFound("No user found"));
        
        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return Result<bool>.Failure(AppError.BadToken("Token is not valid"));
        
        return Result<bool>.Success(result.Succeeded);
    }
}