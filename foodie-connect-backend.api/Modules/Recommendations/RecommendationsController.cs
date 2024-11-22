using foodie_connect_backend.Data;
using foodie_connect_backend.Extensions.DI;
using foodie_connect_backend.Modules.Dishes;
using foodie_connect_backend.Modules.Dishes.Dtos;
using foodie_connect_backend.Modules.Restaurants;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Recommendations;

[Route("v1/recommendations")]
[ApiController]
public class RecommendationsController(RecommendationService recommendationService, RestaurantsService restaurantsService, DishesService dishesService): ControllerBase
{
    [HttpGet("/restaurants")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RestaurantResponseDto>>> GetRecommendedRestaurants(
        string userId, int n
        )
    {
        var restaurantIdsResult = await recommendationService.GetRecommendedRestaurantIdsAsync(userId, n);
        if (!restaurantIdsResult.IsSuccess)
            return BadRequest(restaurantIdsResult.Error);

        var result = new List<RestaurantResponseDto>();
        foreach (var id in restaurantIdsResult.Value)
        {
            var itemResult = await restaurantsService.GetRestaurantById(new Guid(id));
            if(itemResult.IsSuccess)
                result.Add(itemResult.Value);
        }
        
        return Ok(result);
    }
    
    [HttpGet("/dishes")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<RestaurantResponseDto>>> GetRecommendedDishes(
        string userId, int n
    )
    {
        var dishIdsResult = await recommendationService.GetRecommendedDishIdsAsync(userId, n);
        if (!dishIdsResult.IsSuccess)
            return BadRequest(dishIdsResult.Error);

        var result = new List<Dish>();
        foreach (var id in dishIdsResult.Value)
        {
            var itemResult = await dishesService.GetDishById(new Guid(id));
            if(itemResult.IsSuccess)
                result.Add(itemResult.Value);
        }
        
        return Ok(result);
    }
}