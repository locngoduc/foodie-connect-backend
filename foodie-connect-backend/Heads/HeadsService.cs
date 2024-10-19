using foodie_connect_backend.Data;
using foodie_connect_backend.Heads.Dtos;
using foodie_connect_backend.Shared.Classes;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Heads;

public class HeadsService(UserManager<User> userManager)
{
    public async Task<Result<User>> CreateHead(CreateHeadDto head)
    {
        var newHead = new User
        {
            DisplayName = head.DisplayName,
            PhoneNumber = head.PhoneNumber,
            UserName = head.UserName,
            Email = head.Email,
            AvatarUrl = "https://api.dicebear.com/9.x/initials/svg?seed=" + head.UserName
        };
        
        var result = await userManager.CreateAsync(newHead, head.Password);
        await userManager.AddToRoleAsync(newHead, "Head");

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
}