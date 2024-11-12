using System.Security.Claims;
using FluentEmail.Core;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Heads;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Modules.Restaurants.Mappers;
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
        if (result.IsFailure)
            return result.Error.Code switch
            {
                RestaurantError.IncorrectCoordinatesCode => BadRequest(result.Error),
                RestaurantError.DuplicateNameCode => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };

        return CreatedAtAction(nameof(GetRestaurant), new { restaurantId = result.Value.Id }, result.Value);
    }

    [HttpGet("{restaurantId:guid}")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RestaurantResponseDto>> GetRestaurant(Guid restaurantId)
    {
        var result = await restaurantsService.GetRestaurantById(restaurantId);
        if (result.IsFailure) return NotFound(result.Error);
        return Ok(result.Value);
    }


    /// <summary>
    /// Retrieves restaurants within a specified radius of a location
    /// </summary>
    /// <param name="query">Query parameters for filtering restaurants</param>
    /// <returns>A list of restaurants matching the specified criteria</returns>
    /// <response code="200">Returns the list of restaurants within the specified radius</response>
    /// <response code="400">
    /// Bad request
    /// - INCORRECT_COORDINATES: The provided origin string does not follow the specified format
    /// - UNSUPPORTED_QUERY: API does not support querying restaurants in a limitless scope
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RestaurantResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<RestaurantResponseDto>>> GetRestaurants(
        [FromQuery] GetRestaurantsQuery query)
    {
        var result = await restaurantsService.GetRestaurantsQuery(query);

        if (!result.IsSuccess)
            return BadRequest(result.Error);
        
        return Ok(result.Value);
    }


    /// <summary>
    /// Updates an existing restaurant's information
    /// </summary>
    /// <param name="restaurantId">The unique identifier of the restaurant to update</param>
    /// <param name="restaurant">The updated restaurant information</param>
    /// <returns>The updated restaurant information</returns>
    /// <response code="200">Returns the updated restaurant information</response>
    /// <response code="400">
    /// Bad request
    /// - INCORRECT_COORDINATES: The provided coordinates string does not follow the format 'longitude,latitude'
    /// - DUPLICATE_NAME: A restaurant with the provided name already exists
    /// </response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// User is not authorized
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="404">
    /// Not found
    /// - RESTAURANT_NOT_EXIST: The restaurant with the specified ID does not exist
    /// </response>
    /// <response code="500">
    /// Internal server error
    /// - An unexpected error occurred while processing the request
    /// </response>
    [HttpPut("{restaurantId:guid}")]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(RestaurantResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<RestaurantResponseDto>> UpdateRestaurant(
        [FromRoute] Guid restaurantId,
        [FromBody] CreateRestaurantDto restaurant)
    {
        var result = await restaurantsService.UpdateRestaurant(restaurantId, restaurant);

        if (!result.IsSuccess)
        {
            // Map specific error types to appropriate HTTP status codes
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantError.IncorrectCoordinatesCode => BadRequest(result.Error),
                RestaurantError.DuplicateNameCode => BadRequest(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
    
        return Ok(result.Value);
    }


    /// <summary>
    /// Update the restaurant's logo
    /// </summary>
    /// <param name="restaurantId"></param>
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
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="404">
    /// User is not authorized
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    [HttpPut("{restaurantId:guid}/logo")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "RestaurantOwner")]
    public async Task<ActionResult<GenericResponse>> UpdateLogo(Guid restaurantId, IFormFile file)
    {
        var result = await restaurantsService.UploadLogo(restaurantId, file);
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
    /// Update the restaurant's banner
    /// </summary>
    /// <param name="restaurantId"></param>
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
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="404">
    /// User is not authorized
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    [HttpPut("{restaurantId:guid}/banner")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Authorize(Policy = "RestaurantOwner")]
    public async Task<ActionResult<GenericResponse>> UpdateBanner(Guid restaurantId, IFormFile file)
    {
        var result = await restaurantsService.UploadBanner(restaurantId, file);
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
    /// Adds additional images to the restaurant's gallery
    /// </summary>
    /// <param name="restaurantId"></param>
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
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="404">
    /// User is not authorized
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    [HttpPost("{restaurantId:guid}/images")]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GenericResponse>> UploadImages(Guid restaurantId, IFormFile[] files)
    {
        var result = await restaurantsService.UploadImages(restaurantId, files);
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
    /// Delete an image from the restaurant's gallery
    /// </summary>
    /// <param name="restaurantId"></param>
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
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="500">An unexpected error occured. Recheck if image still exist.</response>
    [HttpDelete("{restaurantId:guid}/images/{*imageId}")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = "RestaurantOwner")]
    public async Task<ActionResult<GenericResponse>> DeleteImage(Guid restaurantId, string imageId)
    {
        var result = await restaurantsService.DeleteImage(restaurantId, imageId);
        if (!result.IsFailure) return Ok(new GenericResponse { Message = "Image deleted successfully" });
        
        return result.Error.Code switch
        {
            RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
            RestaurantError.ImageNotExistCode => NotFound(result.Error),
            _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
        };
    }
}