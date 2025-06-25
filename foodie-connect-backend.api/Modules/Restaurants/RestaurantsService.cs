using System.Globalization;
using foodie_connect_backend.Data;
using foodie_connect_backend.Data.Builders;
using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Modules.Restaurants.Mappers;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
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
            var coordinates = restaurant.LongitudeLatitude.Split(',');

            if (coordinates.Length != 2 ||
                !double.TryParse(coordinates[0], NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var longitude) ||
                !double.TryParse(coordinates[1], NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var latitude))
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
                Location = locationPoint,
                HeadId = head.Id,
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

            var response = newRestaurant.ToResponseDto(new ScoreResponseDto());
            return Result<RestaurantResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating restaurant: {ex.Message}");
            return Result<RestaurantResponseDto>.Failure(AppError.InternalError());
        }
    }


    public async Task<Result<RestaurantResponseDto>> UpdateRestaurant(Guid restaurantId, CreateRestaurantDto restaurant)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var existingRestaurant = await dbContext.Restaurants.FindAsync(restaurantId);
            if (existingRestaurant == null) 
                return Result<RestaurantResponseDto>.Failure(RestaurantError.RestaurantNotExist());

            // Check if the new name conflicts with another restaurant (excluding current restaurant)
            var nameExists = await dbContext.Restaurants
                .AnyAsync(r => r.Name.Equals(restaurant.Name) && r.Id != restaurantId);
            if (nameExists)
                return Result<RestaurantResponseDto>.Failure(RestaurantError.DuplicateName(restaurant.Name));

            // Handle coordinates update if provided
            if (!string.IsNullOrEmpty(restaurant.LongitudeLatitude))
            {
                var coordinates = restaurant.LongitudeLatitude.Split(',');

                if (coordinates.Length != 2 ||
                    !double.TryParse(coordinates[0], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out var longitude) ||
                    !double.TryParse(coordinates[1], NumberStyles.Float,
                        CultureInfo.InvariantCulture, out var latitude))
                    return Result<RestaurantResponseDto>.Failure(RestaurantError.IncorrectCoordinates());

                var locationPoint = new Point(longitude, latitude) { SRID = 4326 };
                existingRestaurant.Location = locationPoint;

                // Update area information
                var resultArea = await geoCoderService.GetAddressAsync(latitude, longitude);
                if (resultArea.IsFailure)
                    return Result<RestaurantResponseDto>.Failure(AppError.InternalError());

                await dbContext.Areas.AddAsync(resultArea.Value);
                existingRestaurant.AreaId = resultArea.Value.Id;
            }

            // Update other properties
            existingRestaurant.Name = restaurant.Name;
            existingRestaurant.Phone = restaurant.Phone;
            existingRestaurant.OpenTime = restaurant.OpenTime;
            existingRestaurant.CloseTime = restaurant.CloseTime;
            existingRestaurant.Status = restaurant.Status;

            dbContext.Restaurants.Update(existingRestaurant);
            await dbContext.SaveChangesAsync();
            
            await transaction.CommitAsync();

            var response = existingRestaurant.ToResponseDto(new ScoreResponseDto());
            return Result<RestaurantResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error updating restaurant: {ex.Message}");
            return Result<RestaurantResponseDto>.Failure(AppError.InternalError());
        }
    }

    public async Task<Result<RestaurantResponseDto>> GetRestaurantById(Guid id)
    {
        var restaurant = await dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .Include(r => r.Area)
            .Include(r=>r.Services)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant == null)
            return Result<RestaurantResponseDto>.Failure(RestaurantError.RestaurantNotExist());

        var score = await CalculateRestaurantId(id);
        
        var response = restaurant.ToResponseDto(score);
        return Result<RestaurantResponseDto>.Success(response);
    }

    public async Task<Result<List<RestaurantResponseDto>>> GetRestaurantsQuery(GetRestaurantsQuery queryDetails)
    {
        if (queryDetails.Origin is null && queryDetails.OwnerId is null) 
            return Result<List<RestaurantResponseDto>>.Failure(RestaurantError.UnsupportedQuery());
    
        try
        {
            var query = dbContext.Restaurants.AsQueryable();

            if (queryDetails.Name is not null)
            {
                query = query.Where(r => r.Name.ToLower().Contains(queryDetails.Name.ToLower()));
            }

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
                .Include(r => r.Area)
                .Include(r=>r.Services)
                .ToListAsync();  // Execute the query first

            // Then perform the async mapping
            var restaurantDtos = new List<RestaurantResponseDto>();
            foreach (var restaurant in restaurants)
            {
                var id = await CalculateRestaurantId(restaurant.Id);
                restaurantDtos.Add(restaurant.ToResponseDto(id));
            }

            return Result<List<RestaurantResponseDto>>.Success(restaurantDtos);
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

        if (publicId is "logo" or "banner")
        {
            switch (publicId)
            {
                case "logo":
                {
                    var restaurantLogo = restaurant.Images.FirstOrDefault(i => i.Contains("logo"));
                    if (restaurantLogo != null) await DeleteImage(restaurantId, restaurantLogo);
                    break;
                }
                case "banner":
                {
                    var restaurantBanner = restaurant.Images.FirstOrDefault(i => i.Contains("banner"));
                    if (restaurantBanner != null) await DeleteImage(restaurantId, restaurantBanner);
                    break;
                }
            }
        }

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
    
    public async Task<ScoreResponseDto> CalculateRestaurantId(Guid restaurantId)
    {
        var scores = await CalculateRestaurantScoresAsync(new[] { restaurantId });
        return scores.GetValueOrDefault(restaurantId, new ScoreResponseDto());
    }

    public async Task<Dictionary<Guid, ScoreResponseDto>> CalculateRestaurantScoresAsync(IEnumerable<Guid> restaurantIds)
    {
        var restaurantIdsList = restaurantIds.ToList();
        
        // Get all reviews for the requested dishes in a single query
        var reviewGroups = await dbContext.Set<RestaurantReview>()
            .Where(r => restaurantIdsList.Contains(r.RestaurantId))
            .GroupBy(r => r.RestaurantId)
            .Select(g => new
            {
                DishId = g.Key,
                FiveStars = g.Count(r => r.Rating == 5),
                FourStars = g.Count(r => r.Rating == 4),
                ThreeStars = g.Count(r => r.Rating == 3),
                TwoStars = g.Count(r => r.Rating == 2),
                OneStar = g.Count(r => r.Rating == 1),
                AverageRating = g.Average(r => r.Rating)
            })
            .ToDictionaryAsync(g => g.DishId);

        // Create response dictionary including dishes with no reviews
        var result = new Dictionary<Guid, ScoreResponseDto>();
        foreach (var dishId in restaurantIdsList)
        {
            if (reviewGroups.TryGetValue(dishId, out var group))
            {
                result[dishId] = new ScoreResponseDto
                {
                    FiveStars = group.FiveStars,
                    FourStars = group.FourStars,
                    ThreeStars = group.ThreeStars,
                    TwoStars = group.TwoStars,
                    OneStar = group.OneStar,
                    AverageRating = group.AverageRating
                };
            }
            else
            {
                result[dishId] = new ScoreResponseDto();
            }
        }

        return result;
    }
}