using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using FluentEmail.Core;
using foodie_connect_backend.Data;
using foodie_connect_backend.Heads.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Verification;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Heads;

public class HeadsService(UserManager<User> userManager, VerificationService verificationService, Cloudinary cloudinary)
{
    private readonly List<string> _allowedAvatarExtensions = new List<string> { ".png", ".jpg", ".jpeg", ".webp" };
    public async Task<Result<User>> CreateHead(CreateHeadDto head)
    {
        var newHead = new User
        {
            DisplayName = head.DisplayName,
            PhoneNumber = head.PhoneNumber,
            UserName = head.UserName,
            Email = head.Email,
        };
        
        var result = await userManager.CreateAsync(newHead, head.Password);
        await userManager.AddToRoleAsync(newHead, "Head");
        
        // Send verification email
        // This introduces tight-coupling between Heads and Verification service
        // TODO: Implement a pub/sub that invokes and consumes UserRegistered event
        await verificationService.SendConfirmationEmail(newHead.Id);
        
        if (!result.Succeeded)
        {
            if (result.Errors.FirstOrDefault()?.Code == "DuplicateUserName" ||
                result.Errors.FirstOrDefault()?.Code == "DuplicateEmail")
            {
                return Result<User>.Failure(AppError.Conflict(result.Errors.First().Description));
            }
            return Result<User>.Failure(AppError.ValidationError(result.Errors.First().Description));
        }
        
        return Result<User>.Success(newHead);
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
        if (user == null || !await userManager.IsInRoleAsync(user, "Head"))
            return Result<bool>.Failure(AppError.RecordNotFound("No head is associated with this id"));

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
            Console.WriteLine(uploadResult.PublicId);
            user.AvatarId = uploadResult.PublicId;
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return Result<bool>.Failure(AppError.ValidationError(updateResult.Errors.First().Description));
            return Result<bool>.Success(true);
        }

        Console.WriteLine(uploadResult.Error);
        return Result<bool>.Failure(AppError.InternalError(uploadResult.Error.Message));
    }
}