using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Socials.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.Socials;

public class SocialsService(ApplicationDbContext dbContext)
{

    public async Task<Result<List<SocialLinkResponseDto>>> GetRestaurantSocialLinksAsync(string restaurantId)
    {
        try
        {
            var restaurant = await dbContext.Restaurants
                .Include(r => r.SocialLinks)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                return Result<List<SocialLinkResponseDto>>.Failure(AppError.RecordNotFound("Restaurant not found"));

            var socialLinksResponseDto = restaurant.SocialLinks
            .Select(sl => new SocialLinkResponseDto
            {
                Id = sl.Id,
                PlatformType = sl.PlatformType,
                Url = sl.Url
            }).ToList();
            return Result<List<SocialLinkResponseDto>>.Success(socialLinksResponseDto);
        }
        catch (Exception ex)
        {
            return Result<List<SocialLinkResponseDto>>.Failure(AppError.InternalError($"Error getting social links: {ex.Message}"));
        }
    }

    public async Task<Result<SocialLinkResponseDto>> AddSocialLinkAsync(string restaurantId, CreateSocialDto dto)
    {
        try
        {
            var restaurant = await dbContext.Restaurants
                .Include(r => r.SocialLinks)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                return Result<SocialLinkResponseDto>.Failure(AppError.RecordNotFound("Restaurant not found"));

            if (restaurant.SocialLinks.Any(sl => sl.PlatformType == dto.PlatformType))
                return Result<SocialLinkResponseDto>.Failure(AppError.Conflict($"Social link for {dto.PlatformType} already exists"));

            var socialLink = new SocialLink
            {
                PlatformType = dto.PlatformType,
                Url = dto.Url,
                RestaurantId = restaurantId
            };

            restaurant.SocialLinks.Add(socialLink);
            await dbContext.SaveChangesAsync();

            var socialLinksResponseDto = new SocialLinkResponseDto
            {
                Id = socialLink.Id,
                PlatformType = socialLink.PlatformType,
                Url = socialLink.Url
            };
            return Result<SocialLinkResponseDto>.Success(socialLinksResponseDto);
        }
        catch (Exception ex)
        {
            return Result<SocialLinkResponseDto>.Failure(AppError.InternalError($"Error getting social links: {ex.Message}"));
        }
    }

    public async Task<Result<SocialLinkResponseDto>> UpdateSocialLinkAsync(string restaurantId, UpdateSocialDto dto)
    {
        try
        {
            var socialLink = await dbContext.SocialLinks.FirstOrDefaultAsync(sl => sl.Id == dto.Id && sl.RestaurantId == restaurantId);
            if (socialLink == null)
                return Result<SocialLinkResponseDto>.Failure(AppError.RecordNotFound("Social link not found"));

            var updatedSocialLink = new SocialLink
            {
                Id = dto.Id,
                PlatformType = dto.PlatformType,
                Url = dto.Url,
                RestaurantId = restaurantId
            };
            
            dbContext.ChangeTracker.Clear();
            var result = dbContext.SocialLinks.Update(updatedSocialLink);
            await dbContext.SaveChangesAsync();

            var socialLinksResponseDto = new SocialLinkResponseDto
            {
                Id = updatedSocialLink.Id,
                PlatformType = updatedSocialLink.PlatformType,
                Url = updatedSocialLink.Url,
            };
            return Result<SocialLinkResponseDto>.Success(socialLinksResponseDto);
        }
        catch (Exception ex)
        {
            return Result<SocialLinkResponseDto>.Failure(AppError.InternalError($"Error updating social link: {ex.Message}"));
        }
    }

    public async Task<Result<bool>> DeleteSocialLinkAsync(string restaurantId, string socialLinkId)
    {
        try
        {
            var restaurant = await dbContext.Restaurants
                .Include(r => r.SocialLinks)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                return Result<bool>.Failure(AppError.RecordNotFound("Restaurant not found"));

            var socialLink = restaurant.SocialLinks.FirstOrDefault(sl => sl.Id == socialLinkId);
            if (socialLink == null)
                return Result<bool>.Failure(AppError.RecordNotFound("Social link not found"));

            restaurant.SocialLinks.Remove(socialLink);

            dbContext.SocialLinks.Remove(socialLink);

            await dbContext.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(AppError.InternalError($"Error deleting social link: {ex.Message}"));
        }
    }

}