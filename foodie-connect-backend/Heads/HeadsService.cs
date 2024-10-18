using foodie_connect_backend.Data;
using foodie_connect_backend.Heads.Dtos;
using foodie_connect_backend.Shared.Classes;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Heads;

public class HeadsService(UserManager<Head> userManager)
{
    public async Task<Result<Head>> CreateHead(CreateHeadDto head)
    {
        var newHead = new Head
        {
            DisplayName = head.DisplayName,
            PhoneNumber = head.PhoneNumber,
            UserName = head.UserName,
            Email = head.Email,
        };
        
        var result = await userManager.CreateAsync(newHead, head.Password);

        if (!result.Succeeded)
        {
            if (result.Errors.FirstOrDefault()?.Code == "DuplicateUserName" ||
                result.Errors.FirstOrDefault()?.Code == "DuplicateEmail")
            {
                return Result<Head>.Failure(AppError.Conflict(result.Errors.First().Description));
            }
            return Result<Head>.Failure(AppError.ValidationError(result.Errors.First().Description));
        }
        
        return Result<Head>.Success(newHead);
    }

    public async Task<Result<Head>> GetHeadById(string id)
    {
        var head = await userManager.FindByIdAsync(id);
        if (head == null) return Result<Head>.Failure(AppError.RecordNotFound("No head is associated with this id"));
        return Result<Head>.Success(head);
    }
}