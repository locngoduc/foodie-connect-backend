using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace foodie_connect_backend.Modules.Restaurants;

public class RestaurantsService(
    IUploaderService uploaderService,
    ApplicationDbContext dbContext,
    IGeoCoderService geoCoderService
    )
{
    private RestaurantResponseDto ToResponseDto(Restaurant restaurant)
    {
        return new RestaurantResponseDto
        {
            Id = restaurant.Id,
            Name = restaurant.Name,
            OpenTime = restaurant.OpenTime,
            CloseTime = restaurant.CloseTime,
            Status = restaurant.Status,
            SocialLinks = restaurant.SocialLinks,
            Phone = restaurant.Phone,
            Images = restaurant.Images,
            CreatedAt = restaurant.CreatedAt,
            FormattedAddress = restaurant.Area?.FormattedAddress,
            Latitude = restaurant.Location.Y,    
            Longitude = restaurant.Location.X,   
            HeadId = restaurant.HeadId
        };
    }
    
  public async Task<Result<RestaurantResponseDto>> CreateRestaurant(CreateRestaurantDto restaurant, User head)
{
    await using var transaction = await dbContext.Database.BeginTransactionAsync();
    try
    {
        var coordinates = restaurant.LatitudeLongitude.Split(',');

        if (coordinates.Length != 2 ||
            !double.TryParse(coordinates[0], System.Globalization.NumberStyles.Float, 
                System.Globalization.CultureInfo.InvariantCulture, out var latitude) ||
            !double.TryParse(coordinates[1], System.Globalization.NumberStyles.Float, 
                System.Globalization.CultureInfo.InvariantCulture, out var longitude))
        {
            return Result<RestaurantResponseDto>.Failure(AppError.Conflict("Invalid coordinate format"));
        }

        var locationPoint = new Point(longitude, latitude) { SRID = 4326 };

        var newRestaurant = new Restaurant
        {
            Name = restaurant.Name,
            Phone = restaurant.Phone,
            OpenTime = restaurant.OpenTime,
            CloseTime = restaurant.CloseTime,
            Status = restaurant.Status,
            HeadId = head.Id,
            Location = locationPoint 
        };
        
        var resultArea = await geoCoderService.GetAddressAsync(latitude, longitude);
        if (resultArea.IsFailure)
            return Result<RestaurantResponseDto>.Failure(AppError.InternalError("Error occurred while creating a new area."));

        await dbContext.Areas.AddAsync(resultArea.Value);

        newRestaurant.AreaId = resultArea.Value.Id;

        await dbContext.Restaurants.AddAsync(newRestaurant);
        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
    
        var response = ToResponseDto(newRestaurant);
        return Result<RestaurantResponseDto>.Success(response);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        Console.WriteLine($"Error creating restaurant: {ex.Message}");
        return Result<RestaurantResponseDto>.Failure(AppError.InternalError("Failed to create restaurant"));
    }
}


    public async Task<Result<Restaurant>> UpdateRestaurant(string restaurantId, CreateRestaurantDto restaurant)
    {
        try
        {
            var result = await dbContext.Restaurants.FindAsync(restaurantId);   
            if (result == null) return Result<Restaurant>.Failure(AppError.RecordNotFound("Restaurant not found"));

            var currRestaurant = result;
            currRestaurant.Name = restaurant.Name;
            currRestaurant.Phone = restaurant.Phone;
            currRestaurant.OpenTime = restaurant.OpenTime;
            currRestaurant.CloseTime = restaurant.CloseTime;
            currRestaurant.Status = restaurant.Status;

            await dbContext.Restaurants.AddAsync(currRestaurant);
            await dbContext.SaveChangesAsync();
            return Result<Restaurant>.Success(currRestaurant);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating restaurant: {ex.Message}");
            return Result<Restaurant>.Failure(AppError.InternalError("Failed to update restaurant"));
        }
    }

    public async Task<Result<RestaurantResponseDto>> GetRestaurantById(string id)
    {
        var restaurant = await dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .Include(r=>r.Area)
            .FirstOrDefaultAsync(r => r.Id == id);

        
        if (restaurant == null)
            return Result<RestaurantResponseDto>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"));
        var response = ToResponseDto(restaurant);
        return Result<RestaurantResponseDto>.Success(response);
    }

    private async Task<Result<bool>> UploadImage(string restaurantId, IFormFile file, string publicId,
        string? folder = null)
    {
        var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurant is null)
            return Result<bool>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"));

        var imageOptions = new ImageFileOptions
        {
            PublicId = publicId,
            Folder = !string.IsNullOrEmpty(folder)
                ? $"foodie/restaurants/{restaurantId}/{folder}"
                : $"foodie/restaurants/{restaurantId}/"
        };

        var uploadResult = await uploaderService.UploadImageAsync(file, imageOptions);
        if (uploadResult.IsFailure)
            return Result<bool>.Failure(uploadResult.Error);

        if (!restaurant.Images.Contains(uploadResult.Value)) restaurant.Images.Add(uploadResult.Value);

        try
        {
            await dbContext.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving changes: {ex.Message}");
            return Result<bool>.Failure(AppError.InternalError("Failed to update restaurant's images"));
        }
    }

    public async Task<Result<bool>> DeleteImage(string restaurantId, string imageId)
    {
        if (string.IsNullOrEmpty(restaurantId) || string.IsNullOrEmpty(imageId))
            return Result<bool>.Failure(AppError.ValidationError("Restaurant ID and Image ID are required"));

        var restaurant = await dbContext.Restaurants
            .AsTracking()
            .FirstOrDefaultAsync(r => r.Id == restaurantId);

        if (restaurant == null)
            return Result<bool>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"));

        if (!restaurant.Images.Contains(imageId))
            return Result<bool>.Failure(AppError.RecordNotFound("Image not found"));

        try
        {
            restaurant.Images.Remove(imageId);
            await dbContext.SaveChangesAsync();

            var deleteResult = await uploaderService.DeleteFileAsync(imageId);

            if (deleteResult.IsFailure)
                return Result<bool>.Failure(deleteResult.Error);
            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return Result<bool>.Failure(AppError.InternalError($"Failed to delete image. Ex: {ex.Message}"));
        }
    }

    public Task<Result<bool>> UploadLogo(string restaurantId, IFormFile file)
    {
        return UploadImage(restaurantId, file, "logo");
    }

    public Task<Result<bool>> UploadBanner(string restaurantId, IFormFile file)
    {
        return UploadImage(restaurantId, file, "banner");
    }

    public async Task<Result<bool>> UploadImages(string restaurantId, ICollection<IFormFile> files)
    {
        var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurant == null)
            return Result<bool>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"));

        var uploadTasks = files.Select(file => UploadImage(
            restaurantId,
            file,
            Guid.NewGuid().ToString(),
            "images"
        ));

        var results = await Task.WhenAll(uploadTasks);

        if (results.Any(r => r.IsFailure))
            return Result<bool>.Failure(AppError.ValidationError("Some images failed to upload."));

        return Result<bool>.Success(true);
    }

}