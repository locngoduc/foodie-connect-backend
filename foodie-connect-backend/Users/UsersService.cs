using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using foodie_connect_backend.Data;
using foodie_connect_backend.Users.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Dtos;
using foodie_connect_backend.Verification;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Users;

public class UsersService(UserManager<User> userManager, VerificationService verificationService, Cloudinary cloudinary, SignInManager<User> signInManager)
{
    private readonly List<string> _allowedAvatarExtensions = [".png", ".jpg", ".jpeg", ".webp"];
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
        await userManager.AddToRoleAsync(newUser, "User");
        
        // Send verification email
        // This introduces tight-coupling between Heads and Verification service
        // TODO: Implement a pub/sub that invokes and consumes UserRegistered event
        await verificationService.SendConfirmationEmail(newUser.Id);
        
        if (!result.Succeeded)
        {
            if (result.Errors.FirstOrDefault()?.Code == "DuplicateUserName" ||
                result.Errors.FirstOrDefault()?.Code == "DuplicateEmail")
            {
                return Result<User>.Failure(AppError.Conflict(result.Errors.First().Description));
            }
            return Result<User>.Failure(AppError.ValidationError(result.Errors.First().Description));
        }
        
        return Result<User>.Success(newUser);
    }

    public async Task<Result<User>> GetUserById(string id)
    {
        var head = await userManager.FindByIdAsync(id);
        if (head == null || !await userManager.IsInRoleAsync(head, "User")) 
            return Result<User>.Failure(AppError.RecordNotFound("No user is associated with this id"));
        
        return Result<User>.Success(head);
    }

    public async Task<Result<bool>> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(AppError.RecordNotFound("No user is associated with this id"));
        var result = await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
        {
            switch (result.Errors.First().Code)
            {
                case "PasswordMismatch":
                    return Result<bool>.Failure(AppError.InvalidCredential(result.Errors.First().Description));
                default:
                    return Result<bool>.Failure(AppError.ValidationError(result.Errors.First().Description));
            }
        }
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UploadAvatar(string userId, IFormFile file)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(AppError.RecordNotFound("No user is associated with this id"));

        var extension = Path.GetExtension(file.FileName);
        if (!_allowedAvatarExtensions.Contains(extension))
            return Result<bool>.Failure(AppError.ValidationError($"Allowed file extensions are: {_allowedAvatarExtensions}"));
        
        var fileSize = file.Length;
        if (fileSize < 1 || fileSize > 5 * 1024 * 1024) 
            return Result<bool>.Failure(AppError.ValidationError("Maximum file size is 5MB"));

        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Format = "webp",
            PublicId = userId,
            Folder = "foodie/user_avatars"
        };
        var uploadResult = await cloudinary.UploadAsync(uploadParams);

        if (uploadResult.Error == null)
        {
            user.AvatarId = uploadResult.PublicId;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<bool>.Failure(AppError.ValidationError(updateResult.Errors.First().Description));
            return Result<bool>.Success(true);
        }

        return Result<bool>.Failure(AppError.InternalError(uploadResult.Error.Message));
    }

    public async Task<Result<bool>> UpgradeToHead(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "User"))
            return Result<bool>.Failure(AppError.RecordNotFound("No user is associated with this id"));
    
        try
        {
            var removeResult = await userManager.RemoveFromRoleAsync(user, "User");
            if (!removeResult.Succeeded)
            {
                return Result<bool>.Failure(AppError.InternalError("Failed to remove User role"));
            }

            var addResult = await userManager.AddToRoleAsync(user, "Head");
            if (!addResult.Succeeded)
            {
                // Rollback the remove operation if add fails
                await userManager.AddToRoleAsync(user, "User");
                return Result<bool>.Failure(AppError.InternalError("Failed to add Head role"));
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
            return Result<bool>.Failure(AppError.InvalidCredential($"Role upgrade failed: {ex.Message}"));
        }
    }
}