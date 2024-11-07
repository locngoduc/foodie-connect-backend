using System.Security.Claims;
using foodie_connect_backend.Modules.Heads;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;

namespace foodie_connect_backend.Modules.Restaurants;

[Route("v1/restaurants")]
[ApiController]
public class RestaurantsController(
    RestaurantsService restaurantsService,
    HeadsService headsService
    ) : ControllerBase
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
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var head = await headsService.GetHeadById(userId);
        
        var result = await restaurantsService.CreateRestaurant(restaurantDto, head.Value);
        if (result is { IsFailure: true, Error.Code: RestaurantError.IncorrectCoordinatesCode }) return BadRequest(result.Error);

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



    /// <summary>
    /// Gets restaurants within a specified radius of a location
    /// </summary>
    /// <param name="latitude">Latitude of the center point (-90 to 90)</param>
    /// <param name="longitude">Longitude of the center point (-180 to 180)</param>
    /// <param name="radius">Search radius in meters (default: 5000)</param>
    /// <returns>List of restaurants within the specified radius</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RestaurantResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RestaurantResponseDto>>> GetRestaurants(
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        [FromQuery] double radius = 5000)
    {
        var center = new Point(longitude, latitude) { SRID = 4326 };
    
        var result = await restaurantsService.GetRestaurantsInRadius(center, radius);
    
        if (!result.IsSuccess)
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }


    /// <summary>
    ///     Update the restaurant's logo
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <response code="200">Successfully updated the restaurant's logo</response>
    /// <response code="400">
    /// Image does not meet requirements
    /// - TYPE_NOT_ALLOWED: The image's extension is not allowed, allowed extensions: .png, .jpg, .jpeg, .webp
    /// - EXCEED_MAX_SIZE: Maximum file size is 5MB
    /// </response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// User is not authorized
    /// - NOT_AUTHORIZED: This user is not a HEAD or is trying to change the logo of a restaurant they do not own
    /// </response>
    /// <response code="404">
    /// User is not authorized
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    [HttpPut("{id}/logo")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Head")]
    public async Task<ActionResult<GenericResponse>> UpdateLogo(string id, IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(RestaurantError.RestaurantNotExist(id));
        if (restaurantQuery.Value.HeadId != userId) return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

        var result = await restaurantsService.UploadLogo(restaurantQuery.Value.Id, file);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                UploadError.ExceedMaxSizeCode => BadRequest(result.Error),
                UploadError.TypeNotAllowedCode => BadRequest(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
        
        return Ok(new GenericResponse { Message = "Logo updated successfully" });
    }


    /// <summary>
    ///     Update the restaurant's banner
    /// </summary>
    /// <param name="id"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    /// <response code="200">Successfully updated the restaurant's banner</response>
    /// <response code="400">
    /// Image does not meet requirements
    /// - TYPE_NOT_ALLOWED: The image's extension is not allowed, allowed extensions: .png, .jpg, .jpeg, .webp
    /// - EXCEED_MAX_SIZE: Maximum file size is 5MB
    /// </response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// User is not authorized
    /// - NOT_AUTHORIZED: This user is not a HEAD or is trying to change the banner of a restaurant they do not own
    /// </response>
    /// <response code="404">
    /// User is not authorized
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    [HttpPut("{id}/banner")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Roles = "Head")]
    public async Task<ActionResult<GenericResponse>> UpdateBanner(string id, IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(RestaurantError.RestaurantNotExist(id));
        if (restaurantQuery.Value.HeadId != userId) return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

        var result = await restaurantsService.UploadBanner(restaurantQuery.Value.Id, file);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                UploadError.ExceedMaxSizeCode => BadRequest(result.Error),
                UploadError.TypeNotAllowedCode => BadRequest(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
        
        return Ok(new GenericResponse { Message = "Logo updated successfully" });
    }


    /// <summary>
    ///     Adds additional images to the restaurant's gallery
    /// </summary>
    /// <param name="id"></param>
    /// <param name="files"></param>
    /// <returns></returns>
    /// <response code="200">Successfully uploaded new images to gallery</response>
    /// <response code="400">
    /// Image does not meet requirements
    /// - TYPE_NOT_ALLOWED: The image's extension is not allowed, allowed extensions: .png, .jpg, .jpeg, .webp
    /// - EXCEED_MAX_SIZE: Maximum file size is 5MB
    /// - RESTAURANT_UPLOAD_PARTIAL: Some images failed to upload
    /// </response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// User is not authorized
    /// - NOT_AUTHORIZED: This user is not a HEAD or is trying to change the gallery of a restaurant they do not own
    /// </response>
    /// <response code="404">
    /// User is not authorized
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    [HttpPost("{id}/images")]
    [Authorize(Roles = "Head")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenericResponse>> UploadImages(string id, IFormFile[] files)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        var restaurantQuery = await restaurantsService.GetRestaurantById(id);
        if (restaurantQuery.IsFailure) return NotFound(RestaurantError.RestaurantNotExist(id));
        if (restaurantQuery.Value.HeadId != userId) return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

        var result = await restaurantsService.UploadImages(restaurantQuery.Value.Id, files);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantError.RestaurantUploadPartialErrorCode => BadRequest(result.Error),
                UploadError.ExceedMaxSizeCode => BadRequest(result.Error),
                UploadError.TypeNotAllowedCode => BadRequest(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
        
        return new GenericResponse { Message = "Images uploaded successfully" };
    }


    /// <summary>
    ///     Delete an image from the restaurant's gallery
    /// </summary>
    /// <param name="id"></param>
    /// <param name="imageId"></param>
    /// <returns></returns>
    /// <response code="200">Successfully deleted image from gallery</response>
    /// <response code="400">Malformed request body</response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// User is not authorized
    /// - NOT_AUTHORIZED: This user is not a HEAD or is trying to change the gallery of a restaurant they do not own
    /// </response>
    /// <response code="500">An unexpected error occured. Recheck if image still exist.</response>
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
        if (restaurantQuery.IsFailure) return NotFound(RestaurantError.RestaurantNotExist(id));
        if (restaurantQuery.Value.HeadId != userId) return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

        var result = await restaurantsService.DeleteImage(restaurantQuery.Value.Id, imageId);
        if (!result.IsFailure) return Ok(new GenericResponse { Message = "Image deleted successfully" });
        
        return result.Error.Code switch
        {
            RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
            RestaurantError.ImageNotExistCode => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
}