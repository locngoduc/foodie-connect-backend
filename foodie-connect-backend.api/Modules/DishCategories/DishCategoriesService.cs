using foodie_connect_backend.Data;
using foodie_connect_backend.Data.Builders;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.EntityFrameworkCore;

namespace foodie_connect_backend.Modules.DishCategories;

public class DishCategoriesService(ApplicationDbContext dbContext)
{
    public async Task<Result<DishCategory>> AddDishCategory(Guid restaurantId, string categoryName)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.DishCategories)
            .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
        
        // Checks
        if (restaurant == null) 
            return Result<DishCategory>.Failure(RestaurantError.RestaurantNotExist());
        if (restaurant.DishCategories.Any(x => x.CategoryName == categoryName)) 
            return Result<DishCategory>.Failure(RestaurantError.DishCategoryAlreadyExist(categoryName));

        // Adds dish category to restaurant
        var newDishCategory = new DishCategory
        {
            CategoryName = categoryName,
        };
        
        restaurant.DishCategories.Add(newDishCategory);
        await dbContext.SaveChangesAsync();
        
        return Result<DishCategory>.Success(newDishCategory);
    }

    public async Task<Result<DishCategory>> DeleteDishCategory(Guid restaurantId, string categoryName)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.DishCategories)
            .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
    
        // Checks
        if (restaurant == null) 
            return Result<DishCategory>.Failure(RestaurantError.RestaurantNotExist());
        if (restaurant.DishCategories.All(x => x.CategoryName != categoryName)) 
            return Result<DishCategory>.Failure(RestaurantError.DishCategoryNotExist(categoryName));
    
        // Delete category from dishes
        var dishesWithCategory = await dbContext.Dishes
            .Include(dish => dish.Categories)
            .Where(dish => dish.Categories.Any(dishCategory => dishCategory.CategoryName == categoryName))
            .ToListAsync();
        
        foreach (var dish in dishesWithCategory)
        {
            dish.Categories.Remove(dish.Categories.Single(x => x.CategoryName == categoryName));
        }
    
        // Remove the category from the restaurant
        var categoryToRemove = restaurant.DishCategories.Single(x => x.CategoryName == categoryName);
        restaurant.DishCategories.Remove(categoryToRemove);
    
        // Save changes to database
        await dbContext.SaveChangesAsync();
    
        return Result<DishCategory>.Success(categoryToRemove);
    }

    public async Task<Result<DishCategory[]>> GetDishCategories(Guid restaurantId)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.DishCategories)
            .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
    
        // Checks
        if (restaurant == null) 
            return Result<DishCategory[]>.Failure(RestaurantError.RestaurantNotExist());
        
        return Result<DishCategory[]>.Success(restaurant.DishCategories.ToArray());
    }

    public async Task<Result<DishCategory>> RenameDishCategory(Guid restaurantId, string oldName, string newName)
    {
        var restaurant = await dbContext.Restaurants
            .Include(restaurant => restaurant.DishCategories)
            .FirstOrDefaultAsync(restaurant => restaurant.Id == restaurantId);
    
        // Checks
        if (restaurant == null) 
            return Result<DishCategory>.Failure(RestaurantError.RestaurantNotExist());
        if (restaurant.DishCategories.All(x => x.CategoryName != oldName))
            return Result<DishCategory>.Failure(RestaurantError.DishCategoryNotExist(oldName));
        if (restaurant.DishCategories.Any(x => x.CategoryName == newName))
            return Result<DishCategory>.Failure(RestaurantError.DishCategoryAlreadyExist(newName));
        
        
        var newCategory = new DishCategory { CategoryName = newName, RestaurantId = restaurantId};
        var oldCategory = restaurant.DishCategories.Single(x => x.CategoryName == oldName);
        dbContext.DishCategories.Add(newCategory);
        // Update dishes with old category to include new category
        var dishesWithOldCategory = await dbContext.Dishes
            .Include(dish => dish.Categories)
            .Where(dish => dish.Categories.Any(dishCategory => dishCategory.CategoryName == oldName))
            .ToListAsync();

        foreach (var dish in dishesWithOldCategory)
        {
            dish.Categories.Add(newCategory);
            dish.Categories.Remove(oldCategory);
        }
        
        // Delete old category
        dbContext.DishCategories.Remove(oldCategory);
        await dbContext.SaveChangesAsync();
        
        return Result<DishCategory>.Success(restaurant.DishCategories.Single(x => x.CategoryName == newName));
    }
}