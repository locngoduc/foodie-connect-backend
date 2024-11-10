using System.Globalization;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Modules.Restaurants.Mappers;
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
public async Task<Result<RestaurantResponseDto>> CreateRestaurant(CreateRestaurantDto restaurant, User head)
{
    await using var transaction = await dbContext.Database.BeginTransactionAsync();
    try
    {
        var coordinates = restaurant.LatitudeLongitude.Split(',');

        if (coordinates.Length != 2 ||
            !double.TryParse(coordinates[0], NumberStyles.Float,
                CultureInfo.InvariantCulture, out var latitude) ||
            !double.TryParse(coordinates[1], NumberStyles.Float,
                CultureInfo.InvariantCulture, out var longitude))
            return Result<RestaurantResponseDto>.Failure(RestaurantError.IncorrectCoordinates());

        // Check if a restaurant with the same name already exists
        var existingRestaurant = await dbContext.Restaurants.FirstOrDefaultAsync(r => r.Name.Equals(restaurant.Name));
        if (existingRestaurant != null)
            return Result<RestaurantResponseDto>.Failure(RestaurantError.DuplicateName(restaurant.Name));

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
            return Result<RestaurantResponseDto>.Failure(
                AppError.InternalError());

        await dbContext.Areas.AddAsync(resultArea.Value);

        newRestaurant.AreaId = resultArea.Value.Id;

        await dbContext.Restaurants.AddAsync(newRestaurant);
        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        var response = newRestaurant.ToResponseDto();
        return Result<RestaurantResponseDto>.Success(response);
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync();
        Console.WriteLine($"Error creating restaurant: {ex.Message}");
        return Result<RestaurantResponseDto>.Failure(AppError.InternalError());
    }
}


    public async Task<Result<Restaurant>> UpdateRestaurant(Guid restaurantId, CreateRestaurantDto restaurant)
    {
        try
        {
            var result = await dbContext.Restaurants.FindAsync(restaurantId);
            if (result == null) return Result<Restaurant>.Failure(RestaurantError.RestaurantNotExist());

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
            return Result<Restaurant>.Failure(AppError.InternalError());
        }
    }

    public async Task<Result<RestaurantResponseDto>> GetRestaurantById(Guid id)
    {
        var restaurant = await dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .Include(r => r.Area)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant == null)
            return Result<RestaurantResponseDto>.Failure(RestaurantError.RestaurantNotExist());

        var response = restaurant.ToResponseDto();
        return Result<RestaurantResponseDto>.Success(response);
    }

    public async Task<Result<List<RestaurantResponseDto>>> GetRestaurantsQuery(GetRestaurantsQuery queryDetails)
    {
        if (queryDetails.Origin is null && queryDetails.OwnerId is null) 
            return Result<List<RestaurantResponseDto>>.Failure(RestaurantError.UnsupportedQuery());
        
        try
        {
            var query = dbContext.Restaurants.AsQueryable();

            if (queryDetails.OwnerId is not null)
            {
                query = query.Where(r => r.HeadId == queryDetails.OwnerId);
            }

            if (!string.IsNullOrEmpty(queryDetails.Origin) && queryDetails.Radius.HasValue)
            {
                // Parse origin coordinates from "longitude,latitude" format
                var coords = queryDetails.Origin.Split(',').Select(s => s.Trim()).ToArray();
                if (coords.Length != 2 || 
                    !double.TryParse(coords[0], out var longitude) ||
                    !double.TryParse(coords[1], out var latitude))
                {
                    return Result<List<RestaurantResponseDto>>.Failure(
                        RestaurantError.IncorrectCoordinates());
                }

                var center = new Point(longitude, latitude) { SRID = 4326 };
                var radius = queryDetails.Radius ?? 500;
                
                query = query.Where(r => r.Location.Distance(center) <= radius);
            }

            var restaurants = await query
                .Select(r => r.ToResponseDto())
                .ToListAsync();

            return Result<List<RestaurantResponseDto>>.Success(restaurants);
        }
        catch (Exception ex)
        {
            return Result<List<RestaurantResponseDto>>.Failure(AppError.InternalError());
        }
    }

    private async Task<Result<bool>> UploadImage(Guid restaurantId, IFormFile file, string publicId,
        string? folder = null)
    {
        var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurant is null)
            return Result<bool>.Failure(RestaurantError.RestaurantNotExist());

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
            return Result<bool>.Failure(AppError.InternalError());
        }
    }

    public async Task<Result<bool>> DeleteImage(Guid restaurantId, string imageId)
    {
        var restaurant = await dbContext.Restaurants
            .AsTracking()
            .FirstOrDefaultAsync(r => r.Id == restaurantId);

        if (restaurant == null)
            return Result<bool>.Failure(RestaurantError.RestaurantNotExist());

        if (!restaurant.Images.Contains(imageId))
            return Result<bool>.Failure(RestaurantError.ImageNotExist(imageId));

        try
        {
            restaurant.Images.Remove(imageId);
            await dbContext.SaveChangesAsync();

            var imageDetails = imageId.Split(".");
            var deleteResult = await uploaderService.DeleteFileAsync(imageDetails[0]);
            return deleteResult.IsFailure ? Result<bool>.Failure(deleteResult.Error) : Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting image: {ex.Message}");
            return Result<bool>.Failure(AppError.InternalError());
        }
    }

    public Task<Result<bool>> UploadLogo(Guid restaurantId, IFormFile file)
    {
        return UploadImage(restaurantId, file, "logo");
    }

    public Task<Result<bool>> UploadBanner(Guid restaurantId, IFormFile file)
    {
        return UploadImage(restaurantId, file, "banner");
    }

    public async Task<Result<bool>> UploadImages(Guid restaurantId, ICollection<IFormFile> files)
    {
        var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurant == null)
            return Result<bool>.Failure(RestaurantError.RestaurantNotExist());

        var uploadTasks = files.Select(file => UploadImage(
            restaurantId,
            file,
            Guid.NewGuid().ToString(),
            "images"
        ));

        var results = await Task.WhenAll(uploadTasks);

        if (results.Any(r => r.IsFailure))
            return Result<bool>.Failure(RestaurantError.RestaurantUploadPartialError());

        return Result<bool>.Success(true);
    }
}