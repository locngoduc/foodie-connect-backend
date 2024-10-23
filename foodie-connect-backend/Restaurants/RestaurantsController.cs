using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Heads;
using foodie_connect_backend.Restaurants.Dtos;
using foodie_connect_backend.Shared.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Restaurants;

[Route("v1/restaurants")]
[ApiController]
public class RestaurantsController(RestaurantsService restaurantsService, HeadsService headsService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [Authorize]
    public async Task<IActionResult> CreateRestaurant([FromForm] CreateRestaurantDto restaurantDto)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var head = await headsService.GetHeadById(userId);

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await restaurantsService.CreateRestaurant(restaurantDto, head.Value);

        if (result.IsFailure)
        {
            if (result.Error.Code == AppError.ConflictErrorCode) return Conflict(result.Error);
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetRestaurant), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RestaurantResponseDto>> GetRestaurant([FromRoute] string id)
    {
        var result = await restaurantsService.GetRestaurantById(id);
        if (result.IsFailure) return NotFound(result.Error);
        return Ok(result.Value);
    }
}