using foodie_connect_backend.Data;
using foodie_connect_backend.Data.Builders;
using foodie_connect_backend.Modules.Dishes.Dtos;
using foodie_connect_backend.Modules.Dishes.Strategies;
using foodie_connect_backend.Modules.DishReviews.Dtos;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
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
        var dish = await dbContext.Dishes
            .Include(dish => dish.Categories)
            .Include(dish => dish.PromotionDetails)
                .ThenInclude(details => details.Promotion)
            .FirstOrDefaultAsync(x => x.Id == id);
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

    public async Task<Result<IEnumerable<Dish>>> QueryDishes(GetDishesQuery query)
    {
        Restaurant restaurant = new Restaurant();
        if (query.RestaurantId is not null)
        {
            restaurant = await dbContext.Restaurants
                .Include(restaurant => restaurant.Dishes)
                .ThenInclude(dish => dish.Categories)
                .Include(restaurant => restaurant.Dishes)
                .ThenInclude(dish => dish.PromotionDetails)
                .ThenInclude(detail => detail.Promotion)
                .FirstOrDefaultAsync(x => x.Id == query.RestaurantId);
            if (restaurant == null) return Result<IEnumerable<Dish>>.Failure(DishError.RestaurantNotFound());
        }
        
        var parsedCategories = query.Categories?
            .Split(",")
            .Select(category => category.Trim())
            .Where(category => !string.IsNullOrWhiteSpace(category))
            .ToList();

        IEnumerable<Dish> dishes = new List<Dish>();
        if (query.RestaurantId is not null) dishes = restaurant.Dishes.ToList();
        else dishes = dbContext.Dishes.ToList();
        
        //apply name filter
        if (query.Name is not null)
        {
            dishes = dishes.Where(d => d.Name.ToLower().Contains(query.Name.ToLower()));
        }

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

        // Apply sorting
        if (!string.IsNullOrEmpty(query.SortBy))
        {
            var scores = await CalculateDishScoresAsync(dishes.Select(d => d.Id));
            var sortContext = CreateSortContext(query.SortBy, query.SortAscending, scores);
            dishes = sortContext.Sort(dishes);
        }

        return Result<IEnumerable<Dish>>.Success(dishes);
    }

    private DishSortContext CreateSortContext(string sortBy, bool ascending, Dictionary<Guid, ScoreResponseDto> scores)
    {
        IDishSortStrategy strategy = sortBy.ToLower() switch
        {
            "price" => new PriceSortStrategy(ascending),
            "rating" => new RatingSortStrategy(scores, ascending),
            "name" => new NameSortStrategy(ascending),
            _ => new NameSortStrategy(ascending) // Default to name sorting
        };

        return new DishSortContext(strategy);
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
            .Include(d => d.PromotionDetails)
                .ThenInclude(d => d.Promotion)
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
    

    public async Task<ScoreResponseDto> CalculateDishScoreAsync(Guid dishId)
    {
        var scores = await CalculateDishScoresAsync(new[] { dishId });
        return scores.GetValueOrDefault(dishId, new ScoreResponseDto());
    }

    public async Task<Dictionary<Guid, ScoreResponseDto>> CalculateDishScoresAsync(IEnumerable<Guid> dishIds)
    {
        var dishIdsList = dishIds.ToList();
        
        // Get all reviews for the requested dishes in a single query
        var reviewGroups = await dbContext.Set<DishReview>()
            .Where(r => dishIdsList.Contains(r.DishId))
            .GroupBy(r => r.DishId)
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
        foreach (var dishId in dishIdsList)
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