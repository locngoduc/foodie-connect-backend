using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Dishes.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.Dishes;

public class DishesService(ApplicationDbContext dbContext, IUploaderService uploaderService)
{
    public async Task<Result<Dish>> AddDishToRestaurant(CreateDishDto dto)
    {
        var isNameTaken = await dbContext.Dishes
            .CountAsync(x => x.Name == dto.Name && x.RestaurantId == dto.RestaurantId);
        if (isNameTaken != 0) 
            return Result<Dish>.Failure(DishError.NameAlreadyExists(dto.Name));

        var dish = new Dish
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            RestaurantId = dto.RestaurantId,
        };
        
        var availableCategories = await dbContext.DishCategories
            .Where(x => x.RestaurantId == dto.RestaurantId)
            .ToListAsync();
        
        foreach (var dtoCategory in dto.Categories)
        {
            if (availableCategories.Any(x => x.CategoryName == dtoCategory)) 
                dish.Categories.Add(availableCategories.Single(x => x.CategoryName == dtoCategory));
        }

        dbContext.Dishes.Add(dish);
        await dbContext.SaveChangesAsync();
        return Result<Dish>.Success(dish);
    }

    public async Task<Result<Dish>> GetDishById(Guid id)
    {
        var dish = await dbContext.Dishes.FindAsync(id);
        if (dish is null) return Result<Dish>.Failure(DishError.DishNotFound());
        
        return Result<Dish>.Success(dish);
    }

    public async Task<Result<bool>> DeleteDish(Guid dishId)
    {
        var dish = await dbContext.Dishes.FindAsync(dishId);
        if (dish == null) return Result<bool>.Failure(DishError.DishNotFound());
        
        // Delete image
        var fileDetails = dish.ImageId?.Split(".");
        if (fileDetails is { Length: > 1 }) await uploaderService.DeleteFileAsync(fileDetails[0]);
        
        // Delete dish from database
        dbContext.Dishes.Remove(dish);
        await dbContext.SaveChangesAsync();
        
        return Result<bool>.Success(true);
    }

    public async Task<Result<List<Dish>>> QueryDishes(GetDishesQuery query)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.Dishes)
            .ThenInclude(dish => dish.Categories)
            .FirstOrDefaultAsync(x => x.Id == query.RestaurantId);
        if (restaurant == null) return Result<List<Dish>>.Failure(DishError.RestaurantNotFound());
        
        var parsedCategories = query.Categories?
            .Split(",")
            .Select(category => category.Trim())
            .Where(category => !string.IsNullOrWhiteSpace(category))
            .ToList();

        var dishes = restaurant.Dishes.ToList();

        // Apply category filter
        if (parsedCategories != null && parsedCategories.Count != 0)
        {
            dishes = dishes.Where(dish => dish.Categories
                .Any(dishCategory => parsedCategories
                    .Any(parsedCategory => dishCategory.CategoryName
                        .Equals(parsedCategory, StringComparison.CurrentCultureIgnoreCase))))
                .ToList();
        }
        
        // Apply price filters
        if (query.MinPrice != null)
            dishes = dishes.Where(dish => dish.Price >= query.MinPrice).ToList();
        if (query.MaxPrice != null)
            dishes = dishes.Where(dish => dish.Price <= query.MaxPrice).ToList();

        return Result<List<Dish>>.Success(dishes);
    }

    public async Task<Result<string>> SetDishImage(Guid dishId, IFormFile file)
    {
        // Check dish existence
        var dish = await dbContext.Dishes.FindAsync(dishId);
        if (dish is null) return Result<string>.Failure(DishError.DishNotFound());

        // Upload image
        var uploadOptions = new ImageFileOptions
        {
            Folder = $"foodie/dishes",
            PublicId = dishId.ToString(),
            Format = "webp"
        };
        var uploadResult = await uploaderService.UploadImageAsync(file, uploadOptions);
        if (uploadResult.IsFailure) return Result<string>.Failure(uploadResult.Error);
        
        // Update dish ImageId reference
        dbContext.Update(dish);
        dish.ImageId = uploadResult.Value;
        await dbContext.SaveChangesAsync();
        
        return Result<string>.Success(dish.ImageId);
    }

    public async Task<Result<Dish>> UpdateDish(Guid dishId, UpdateDishDto dto)
    {
        var dish = await dbContext.Dishes
            .Include(d => d.Categories)
            .FirstOrDefaultAsync(d => d.Id == dishId);
        
        if (dish is null) 
            return Result<Dish>.Failure(DishError.DishNotFound());

        var isNameTaken = await dbContext.Dishes
            .CountAsync(x => x.Name == dto.Name && x.RestaurantId == dish.RestaurantId && x.Id != dishId);
        if (isNameTaken != 0) 
            return Result<Dish>.Failure(DishError.NameAlreadyExists(dto.Name));
    
        var availableCategories = await dbContext.DishCategories
            .Where(x => x.RestaurantId == dish.RestaurantId)
            .ToListAsync(); 
        
        dish.Name = dto.Name;
        dish.Description = dto.Description;
        dish.Price = dto.Price;
        dish.Categories.Clear();
        
        foreach (var categoryName in dto.Categories)
        {
            var category = availableCategories.FirstOrDefault(x => x.CategoryName == categoryName);
            if (category != null && !dish.Categories.Contains(category))
            {
                dish.Categories.Add(category);
            }
        }
    
        await dbContext.SaveChangesAsync();
        return Result<Dish>.Success(dish);
    }
}