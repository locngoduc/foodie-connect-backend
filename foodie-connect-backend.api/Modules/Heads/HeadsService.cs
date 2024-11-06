using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Heads.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Modules.Heads;

public class HeadsService(
    UserManager<User> userManager,
    VerificationService verificationService,
    IUploaderService uploaderService)
{
    public async Task<Result<User>> CreateHead(CreateHeadDto head)
    {
        var newHead = new User
        {
            DisplayName = head.DisplayName,
            PhoneNumber = head.PhoneNumber,
            UserName = head.UserName,
            Email = head.Email
        };

        var result = await userManager.CreateAsync(newHead, head.Password);
        if (!result.Succeeded)
        {
            if (result.Errors.FirstOrDefault()?.Code == "DuplicateUserName" ||
                result.Errors.FirstOrDefault()?.Code == "DuplicateEmail")
                return Result<User>.Failure(AppError.Conflict(result.Errors.First().Description));
            return Result<User>.Failure(AppError.ValidationError(result.Errors.First().Description));
        }

        var createdUser = await userManager.FindByIdAsync(newHead.Id);
        if (createdUser == null) throw new Exception("User was created but cannot be found");

        // Add role
        var roleResult = await userManager.AddToRoleAsync(createdUser, "Head");
        if (!roleResult.Succeeded)
            throw new Exception(
                $"Failed to assign role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

        // Send verification email
        // This introduces tight-coupling between Heads and Verification service
        // TODO: Implement a pub/sub that invokes and consumes UserRegistered event
        try
        {
            await verificationService.SendConfirmationEmail(createdUser.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send verification email: {ex.Message}");
        }

        return Result<User>.Success(createdUser);
    }

    public async Task<Result<User>> GetHeadById(string id)
    {
        var head = await userManager.FindByIdAsync(id);
        if (head == null || !await userManager.IsInRoleAsync(head, "Head"))
            return Result<User>.Failure(AppError.RecordNotFound("No head is associated with this id"));

        return Result<User>.Success(head);
    }

    public async Task<Result<bool>> ChangePassword(string userId, ChangePasswordDto changePasswordDto)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "Head"))
            return Result<bool>.Failure(AppError.RecordNotFound("No head is associated with this id"));
        var result =
            await userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);
        if (!result.Succeeded)
            switch (result.Errors.First().Code)
            {
                case "PasswordMismatch":
                    return Result<bool>.Failure(AppError.InvalidCredential(result.Errors.First().Description));
                default:
                    return Result<bool>.Failure(AppError.ValidationError(result.Errors.First().Description));
            }

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UploadAvatar(string userId, IFormFile file)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null || !await userManager.IsInRoleAsync(user, "Head"))
            return Result<bool>.Failure(AppError.RecordNotFound("No head is associated with this id"));

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
                return Result<bool>.Failure(AppError.ValidationError(updateResult.Errors.First().Description));
            return Result<bool>.Success(true);
        }

        return Result<bool>.Failure(uploadResult.Error);
    }
}