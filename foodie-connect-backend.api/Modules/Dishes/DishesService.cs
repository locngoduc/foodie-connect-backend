using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Dishes.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;

namespace foodie_connect_backend.Modules.Dishes;

public class DishesService(ApplicationDbContext dbContext, IUploaderService uploaderService)
{
    public async Task<Result<Dish>> CreateDish(CreateDishDto dish)
    {
        var newDish = new Dish
        {
            Name = dish.Name,
            Description = dish.Description,
            Price = dish.Price,
            RestaurantId = dish.RestaurantId
        };

        try
        {
            await dbContext.Dishes.AddAsync(newDish);
            await dbContext.SaveChangesAsync();
            return Result<Dish>.Success(newDish);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating dish: {ex.Message}");
            return Result<Dish>.Failure(AppError.InternalError("Failed to create dish"));
        }
    }

    public async Task<Result<Dish>> GetDishById(string id)
    {
        var dish = await dbContext.Dishes.FindAsync(id);

        if (dish == null)
            return Result<Dish>.Failure(DishError.DishNotFound(id));
        return Result<Dish>.Success(dish);
    }

    public async Task<Result<Dish>> UpdateDish(string dishId, CreateDishDto dish)
    {
        try
        {
            var result = await GetDishById(dishId);
            if (!result.IsSuccess)
                return result;

            var currDish = result.Value;
            currDish.Name = dish.Name;
            currDish.Description = dish.Description;
            currDish.Price = dish.Price;

            await dbContext.SaveChangesAsync();
            return Result<Dish>.Success(currDish);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating dish: {ex.Message}");
            return Result<Dish>.Failure(DishError.DishNotFound(dishId));
        }
    }

    public async Task<Result<bool>> DeleteDish(string dishId)
    {
        try
        {
            var result = await GetDishById(dishId);
            if (!result.IsSuccess)
                return Result<bool>.Failure(DishError.DishNotFound(dishId));

            var dish = result.Value;
            dish.IsDeleted = true;
            await dbContext.SaveChangesAsync();
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Result<bool>.Failure(AppError.InternalError("Failed to delete dish"));
        }
    }

    public async Task<Result<bool>> UploadImage(string dishId, IFormFile file)
    {
        try
        {
            var result = await GetDishById(dishId);
            if (!result.IsSuccess)
                return Result<bool>.Failure(DishError.DishNotFound(dishId));

            var dish = result.Value;

            var imageOptions = new ImageFileOptions
            {
                PublicId = "image",
                Folder = $"foodie/dishes/{dishId}/"
            };

            var uploadResult = await uploaderService.UploadImageAsync(file, imageOptions);
            if (uploadResult.IsFailure)
                return Result<bool>.Failure(uploadResult.Error);

            dish.ImageId = uploadResult.Value;

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return Result<bool>.Failure(AppError.InternalError("Failed to upload image"));
        }
    }
}