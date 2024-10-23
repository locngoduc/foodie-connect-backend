

using System.Security.Claims;
using foodie_connect_backend.Restaurants;
using foodie_connect_backend.SocialLinks.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace foodie_connect_backend.SocialLinks;

[ApiController]
[Route("v1/restaurants/{restaurantId}/social-links")]
public class SocialLinksController(SocialLinksService socialLinksService, RestaurantsService restaurantsService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<SocialLinkResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRestaurantSocialLinks(string restaurantId)
    {
        var result = await socialLinksService.GetRestaurantSocialLinksAsync(restaurantId);
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    [ProducesResponseType(typeof(SocialLinkResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> AddSocialLink(string restaurantId, CreateSocialLinkDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var authorizationResult = await CheckRestaurantAuthorization(restaurantId, userId);
        if (authorizationResult != null)
        {
            return authorizationResult;
        }

        var result = await socialLinksService.AddSocialLinkAsync(restaurantId, dto);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return CreatedAtAction(
            nameof(GetRestaurantSocialLinks),
            new { restaurantId },
            result.Value
        );
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(SocialLinkResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> UpdateSocialLink(
        string restaurantId,
        string id,
        UpdateSocialLinkDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var authorizationResult = await CheckRestaurantAuthorization(restaurantId, userId);
        if (authorizationResult != null)
        {
            return authorizationResult;
        }
        if (id != dto.Id) return BadRequest("ID mismatch");

        var result = await socialLinksService.UpdateSocialLinkAsync(restaurantId, dto);
        if (!result.IsSuccess) return NotFound(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> DeleteSocialLink(string restaurantId, string id)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var authorizationResult = await CheckRestaurantAuthorization(restaurantId, userId);
        if (authorizationResult != null)
        {
            return authorizationResult;
        }
        var result = await socialLinksService.DeleteSocialLinkAsync(restaurantId, id);
        if (!result.IsSuccess) return NotFound(result.Error);

        return NoContent();
    }

    private async Task<IActionResult> CheckRestaurantAuthorization(string restaurantId, string userId)
    {
        var restaurant = await restaurantsService.GetRestaurantById(restaurantId);
        if (restaurant.IsFailure)
        {
            return NotFound("Restaurant not found");
        }
        if (restaurant.Value.HeadId != userId)
        {
            return Forbid("You do not have permission to modify this restaurant's resources");
        }
        return null;
    }
}
