using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Heads;
using foodie_connect_backend.Restaurants.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Dtos;
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
    [Consumes("application/json")]
    [Produces("application/json")]
    [Authorize(Roles = "Head")]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantDto restaurantDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
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
    public async Task<ActionResult<Restaurant>> GetRestaurant([FromRoute] string id)
    {
        var result = await restaurantsService.GetRestaurantById(id);
        if (result.IsFailure) return NotFound(result.Error);
        return Ok(result.Value);
    }



    /// <summary>
    /// Update the restaurant's logo
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <response code="200">Successfully updated the restaurant's logo</response>
    /// <response code="400">Malformed request body</response>
    /// <response code="401">Restaurant id not found</response>
    /// <response code="403">Not a HEAD account or HEAD account does not own this restaurant</response>
    [HttpPut("{id}/logo")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Head")]
    public async Task<ActionResult<GenericResponse>> UpdateLogo(string id, IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(restaurantQuery.Error);
        if (restaurantQuery.Value.HeadId != userId) return Forbid();

        var result = await restaurantsService.UploadLogo(restaurantQuery.Value.Id, file);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok(new GenericResponse { Message = "Logo updated successfully" });
    }



    /// <summary>
    /// Update the restaurant's banner
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <response code="200">Successfully updated the restaurant's banner</response>
    /// <response code="400">Malformed request body</response>
    /// <response code="401">Restaurant id not found</response>
    /// <response code="403">Not a HEAD account or HEAD account does not own this restaurant</response>
    [HttpPut("{id}/banner")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [Authorize(Roles = "Head")]
    public async Task<ActionResult<GenericResponse>> UpdateBanner(string id, IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(restaurantQuery.Error);
        if (restaurantQuery.Value.HeadId != userId) return Forbid();

        var result = await restaurantsService.UploadBanner(restaurantQuery.Value.Id, file);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok(new GenericResponse { Message = "Logo updated successfully" });
    }



    /// <summary>
    /// Adds additional images to the restaurant's gallery
    /// </summary>
    /// <param name="id"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    /// <response code="200">Successfully uploaded new images to gallery</response>
    /// <response code="400">Malformed request body</response>
    /// <response code="401">Restaurant id not found</response>
    /// <response code="403">Not a HEAD account or HEAD account does not own this restaurant</response>
    [HttpPost("{id}/images")]
    [Authorize(Roles = "Head")]
    public async Task<ActionResult<GenericResponse>> UploadImages(string id, IFormFile[] files)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(restaurantQuery.Error);
        Console.WriteLine($"Restaurant owner: {restaurantQuery.Value.HeadId}");
        Console.WriteLine($"Requester: {userId}");
        if (restaurantQuery.Value.HeadId != userId) return Forbid();

        var result = await restaurantsService.UploadImages(restaurantQuery.Value.Id, files);
        if (result.IsFailure) return BadRequest(result.Error);
        return Ok(new GenericResponse { Message = "Images uploaded successfully" });
    }


    /// <summary>
    /// Delete an image from the restaurant's gallery
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imageId"></param>
    /// <returns></returns>
    /// <response code="200">Successfully deleted image from gallery</response>
    /// <response code="400">Malformed request body</response>
    /// <response code="401">Restaurant id not found</response>
    /// <response code="403">Not a HEAD account or HEAD account does not own this restaurant</response>
    /// <response code="500">An unexpected error occured. The image may still have been deleted.</response>
    [HttpDelete("{id}/images/{*imageId}")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Head")]
    public async Task<ActionResult<GenericResponse>> DeleteImage(string id, string imageId)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(restaurantQuery.Error);
        if (restaurantQuery.Value.HeadId != userId) return Forbid();

        var result = await restaurantsService.DeleteImage(restaurantQuery.Value.Id, imageId);
        if (result.IsFailure) return result.Error.Code switch
        {
            "RecordNotFound" => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
        return Ok(new GenericResponse { Message = "Image deleted successfully" });
    }
}