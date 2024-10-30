using foodie_connect_backend.Data;
using foodie_connect_backend.Dishes.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Uploader;

namespace foodie_connect_backend.Dishes;

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
            return Result<Dish>.Failure(AppError.RecordNotFound("No dish is associated with this id"));
        return Result<Dish>.Success(dish);
    }

    public async Task<Result<Dish>> UpdateDish(string dishId, CreateDishDto dish)
    {
        try
        {
            var result = await GetDishById(dishId);
            if (!result.IsSuccess)
                return Result<Dish>.Failure(AppError.RecordNotFound("No dish is associated with this id"));

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
            return Result<Dish>.Failure(AppError.InternalError("Failed to update dish"));
        }
    }
}