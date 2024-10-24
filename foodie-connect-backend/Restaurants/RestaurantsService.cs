using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using foodie_connect_backend.Data;
using foodie_connect_backend.Restaurants.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.SocialLinks;
using foodie_connect_backend.SocialLinks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Restaurants;

public class RestaurantsService(Cloudinary cloudinary, ApplicationDbContext dbContext, SocialLinksService socialLinksService)
{
    private readonly List<string> _allowedAvatarExtensions = new() { ".png", ".jpg", ".jpeg", ".webp" };

    public async Task<Result<Restaurant>> CreateRestaurant(CreateRestaurantDto restaurant, User head)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var newRestaurant = new Restaurant
            {
                Name = restaurant.Name,
                Phone = restaurant.Phone,
                OpenTime = restaurant.OpenTime,
                CloseTime = restaurant.CloseTime,
                Address = restaurant.Address,
                Status = restaurant.Status,
                HeadId = head.Id
            };

            await dbContext.Restaurants.AddAsync(newRestaurant);
            await dbContext.SaveChangesAsync();


            await transaction.CommitAsync();

            return Result<Restaurant>.Success(newRestaurant);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Error creating restaurant: {ex.Message}");
            return Result<Restaurant>.Failure(AppError.InternalError("Failed to create restaurant"));
        }
    }

    public async Task<Result<Restaurant>> GetRestaurantById(string id)
    {
        var restaurant = await dbContext.Restaurants
            .Include(r => r.SocialLinks)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (restaurant == null)
            return Result<Restaurant>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"));

        return restaurant == null
            ? Result<Restaurant>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"))
            : Result<Restaurant>.Success(restaurant);
    }

    private async Task<Result<bool>> ValidateAndGetRestaurant(string restaurantId, IFormFile file)
    {
        var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurant == null)
            return Result<bool>.Failure(AppError.RecordNotFound("No restaurant is associated with this id"));

        var extension = Path.GetExtension(file.FileName);
        if (!_allowedAvatarExtensions.Contains(extension))
            return Result<bool>.Failure(
                AppError.ValidationError(
                    $"Allowed file extensions are: {string.Join(", ", _allowedAvatarExtensions)}"));

        if (file.Length < 1 || file.Length > 5 * 1024 * 1024)
            return Result<bool>.Failure(AppError.ValidationError("Maximum file size is 5MB"));

        return Result<bool>.Success(true);
    }

    private async Task<Result<bool>> UploadImage(string restaurantId, IFormFile file, string publicId,
        string? folder = null)
    {
        var validationResult = await ValidateAndGetRestaurant(restaurantId, file);
        if (validationResult.IsFailure)
            return validationResult;

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            Format = "webp",
            PublicId = publicId
        };

        if (!string.IsNullOrEmpty(folder))
            uploadParams.Folder = $"foodie/restaurants/{restaurantId}/{folder}";
        else
            uploadParams.Folder = $"foodie/restaurants/{restaurantId}/";

        var uploadResult = await cloudinary.UploadAsync(uploadParams);
        if (uploadResult.Error != null)
        {
            Console.WriteLine(uploadResult.Error);
            return Result<bool>.Failure(AppError.InternalError(uploadResult.Error.Message));
        }
        Console.WriteLine("Image uploaded to cloudinary");
        var restaurant = await dbContext.Restaurants.FindAsync(restaurantId);
        if (restaurant!.Images.Contains(uploadResult.PublicId))
        {
            Console.WriteLine("Adding image to restaurant");
            restaurant!.Images.Remove(uploadResult.PublicId);
        }
        restaurant!.Images.Add(uploadResult.PublicId);
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

            var deleteParams = new DeletionParams(imageId);
            var deleteResult = await cloudinary.DestroyAsync(deleteParams);

            if (deleteResult.Error != null)
                return Result<bool>.Failure(AppError.InternalError(deleteResult.Error.Message));

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