using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Modules.Users.Dtos;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Modules.Users;

public class UsersService(
    UserManager<User> userManager, 
    VerificationService verificationService, 
    IUploaderService uploaderService, 
    SignInManager<User> signInManager)
{
    public async Task<Result<User>> CreateUser(CreateUserDto user)
    {
        var newUser = new User
        {
            DisplayName = user.DisplayName,
            PhoneNumber = user.PhoneNumber,
            UserName = user.UserName,
            Email = user.Email,
        };
        
        var result = await userManager.CreateAsync(newUser, user.Password);
        
        if (!result.Succeeded)
        {
            return result.Errors.FirstOrDefault()?.Code switch
            {
                nameof(IdentityErrorDescriber.DuplicateUserName) => Result<User>.Failure(UserError.DuplicateUsername(user.UserName)),
                nameof(IdentityErrorDescriber.DuplicateEmail) => Result<User>.Failure(UserError.DuplicateEmail(user.Email)),
                nameof(IdentityErrorDescriber.PasswordTooShort) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresDigit) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresLower) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresUpper) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                _ => Result<User>.Failure(AppError.UnexpectedError(result.Errors.First().Description))
            };
        }
        
        await userManager.AddToRoleAsync(newUser, "User");
        
        // Send verification email
        // This introduces tight-coupling between Heads and Verification service
        // TODO: Implement a pub/sub that invokes and consumes UserRegistered event
        try
        {
            await verificationService.SendConfirmationEmail(newUser.Id);
        }
        catch
        {
            Console.WriteLine("Email verification failed");
        }
        
        return Result<User>.Success(newUser);
    }

    public async Task<Result<User>> GetUserById(string id)
    {
        var head = await userManager.FindByIdAsync(id);
        if (head == null || !await userManager.IsInRoleAsync(head, "User")) 
            return Result<User>.Failure(UserError.UserNotFound(id));
        
        return Result<User>.Success(head);
    }

    public async Task<Result<bool>> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(UserError.UserNotFound(userId));
        
        var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            return result.Errors.FirstOrDefault()?.Code switch
            {
                nameof(IdentityErrorDescriber.PasswordTooShort) => Result<bool>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresDigit) => Result<bool>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresLower) => Result<bool>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresUpper) => Result<bool>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) => Result<bool>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordMismatch) => Result<bool>.Failure(UserError.PasswordMismatch()),
                _ => Result<bool>.Failure(AppError.UnexpectedError(result.Errors.First().Description))
            };
        }
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UploadAvatar(string userId, IFormFile file)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(UserError.UserNotFound(userId));

        var uploadParams = new ImageFileOptions()
        {
            Format = "webp",
            PublicId = userId,
            Folder = "foodie/user_avatars"
        };
        var uploadResult = await uploaderService.UploadImageAsync(file, uploadParams);

        if (uploadResult.IsSuccess)
        {
            user.AvatarId = uploadResult.Value;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<bool>.Failure(AppError.InternalError());
            
            return Result<bool>.Success(true);
        }

        return Result<bool>.Failure(uploadResult.Error);
    }

    public async Task<Result<bool>> UpgradeToHead(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(UserError.UserNotFound(userId));
    
        try
        {
            var removeResult = await userManager.RemoveFromRoleAsync(user, "User");
            if (!removeResult.Succeeded)
            {
                return Result<bool>.Failure(AppError.InternalError());
            }

            var addResult = await userManager.AddToRoleAsync(user, "Head");
            if (!addResult.Succeeded)
            {
                // Rollback the remove operation if add fails
                await userManager.AddToRoleAsync(user, "User");
                return Result<bool>.Failure(AppError.InternalError());
            }

            await signInManager.SignOutAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            // Ensure user still has User role if any exception occurs
            if (!await userManager.IsInRoleAsync(user, "User"))
            {
                await userManager.AddToRoleAsync(user, "User");
            }
            
            return Result<bool>.Failure(AppError.InternalError());
        }
    }
}