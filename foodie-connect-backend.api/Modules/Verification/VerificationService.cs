using FluentEmail.Core;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Modules.Verification;

public class VerificationService(UserManager<User> userManager, IFluentEmail fluentEmail)
{
    public async Task<Result<bool>> SendConfirmationEmail(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result<bool>.Failure(VerificationError.UserNotFound(userId));
        if (user.EmailConfirmed) return Result<bool>.Failure(VerificationError.EmailAlreadyConfirmed());

        // Send verification email
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        string htmlTemplate = await File.ReadAllTextAsync("./foodie-connect-backend.api/Modules/Verification/Templates/EmailVerification.html");
        htmlTemplate = htmlTemplate
            .Replace("{token}", token);
        var email = await fluentEmail
            .To(user.Email)
            .SetFrom("verify@account.foodie.town", "The Foodie team")
            .Subject("Verify your foodie town account")
            .Body(htmlTemplate, true)
            .SendAsync();

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ConfirmEmail(string userId, string token)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Result<bool>.Failure(VerificationError.UserNotFound(userId));

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return Result<bool>.Failure(VerificationError.EmailVerificationTokenInvalid());

        return Result<bool>.Success(result.Succeeded);
    }
}