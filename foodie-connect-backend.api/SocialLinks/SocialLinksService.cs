

using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.SocialLinks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.SocialLinks;

public class SocialLinksService(ApplicationDbContext dbContext)
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
                Platform = sl.Platform,
                Url = sl.Url
            }).ToList();
            return Result<List<SocialLinkResponseDto>>.Success(socialLinksResponseDto);
        }
        catch (Exception ex)
        {
            return Result<List<SocialLinkResponseDto>>.Failure(AppError.InternalError($"Error getting social links: {ex.Message}"));
        }
    }

    public async Task<Result<SocialLinkResponseDto>> AddSocialLinkAsync(string restaurantId, CreateSocialLinkDto dto)
    {
        try
        {
            var restaurant = await dbContext.Restaurants
                .Include(r => r.SocialLinks)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);

            if (restaurant == null)
                return Result<SocialLinkResponseDto>.Failure(AppError.RecordNotFound("Restaurant not found"));

            if (restaurant.SocialLinks.Any(sl => sl.Platform == dto.Platform))
                return Result<SocialLinkResponseDto>.Failure(AppError.Conflict($"Social link for {dto.Platform} already exists"));

            var socialLink = new SocialLink
            {
                Platform = dto.Platform,
                Url = dto.Url,
                RestaurantId = restaurantId
            };

            restaurant.SocialLinks.Add(socialLink);
            await dbContext.SaveChangesAsync();

            var socialLinksResponseDto = new SocialLinkResponseDto
            {
                Id = socialLink.Id,
                Platform = socialLink.Platform,
                Url = socialLink.Url
            };
            return Result<SocialLinkResponseDto>.Success(socialLinksResponseDto);
        }
        catch (Exception ex)
        {
            return Result<SocialLinkResponseDto>.Failure(AppError.InternalError($"Error getting social links: {ex.Message}"));
        }
    }

    public async Task<Result<SocialLinkResponseDto>> UpdateSocialLinkAsync(string restaurantId, UpdateSocialLinkDto dto)
    {
        try
        {
            var restaurant = await dbContext.Restaurants
                .Include(r => r.SocialLinks)
                .FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null)
                return Result<SocialLinkResponseDto>.Failure(AppError.RecordNotFound("Restaurant not found"));

            var socialLink = restaurant.SocialLinks.FirstOrDefault(sl => sl.Id == dto.Id);
            if (socialLink == null)
                return Result<SocialLinkResponseDto>.Failure(AppError.RecordNotFound("Social link not found"));

            if (restaurant.SocialLinks.Any(sl => sl.Platform == dto.Platform))
                return Result<SocialLinkResponseDto>.Failure(AppError.Conflict($"Social link for {dto.Platform} already exists"));

            var updatedSocialLink = new SocialLink
            {
                Id = socialLink.Id,
                Platform = dto.Platform,
                Url = dto.Url
            };

            restaurant.SocialLinks.Remove(socialLink);
            restaurant.SocialLinks.Add(updatedSocialLink);
            await dbContext.SaveChangesAsync();

            var socialLinksResponseDto = new SocialLinkResponseDto
            {
                Id = socialLink.Id,
                Platform = socialLink.Platform,
                Url = socialLink.Url
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