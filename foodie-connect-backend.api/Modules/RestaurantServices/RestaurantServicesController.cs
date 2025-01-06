using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.RestaurantServices.Dtos;
using foodie_connect_backend.Modules.RestaurantServices.Mapper;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.RestaurantServices;

[Route("v1/restaurants/{restaurantId:guid}/services")]
[ApiController]
public class RestaurantServicesController(RestaurantServicesService restaurantServicesService): ControllerBase
{
    /// <summary>
    /// Adds a service to restaurant
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="restaurantId"></param>
    /// <returns>The newly added category</returns>
    /// <response code="200">Successfully added the new service</response>
    /// <response code="400">Request body is invalid</response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// Insufficient permission
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="404">
    /// Service not found
    /// - SERVICE_NOT_FOUND: Service not found
    /// </response>
    /// <response code="409">
    /// Conflict with existing service
    /// - SERVICE_ALREADY_EXISTS: Already added this service
    /// </response>
    [HttpPost]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(RestaurantServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<RestaurantServiceResponseDto>> AddRestaurantService(Guid restaurantId, CreateRestaurantServiceDto body)
    {
        var result = await restaurantServicesService.AddRestaurantService(restaurantId, body);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantServiceError.ServiceConflictCode => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
        return Ok(result.Value.ToResponseDto());
    }

    /// <summary>
    /// Get a list of services in the restaurant
    /// </summary>
    /// <param name="restaurantId"></param>
    /// <returns>A list of services</returns>
    /// <response code="200">Successfully queried the restaurant's services</response>
    /// <response code="404">
    /// Restaurant not found
    /// - RESTAURANT_NOT_EXIST: The provided ID does not correspond to any restaurant
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(RestaurantServiceResponseDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Service[]>> GetRestaurantServices(Guid restaurantId)
    {
        var result = await restaurantServicesService.GetRestaurantServices(restaurantId);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }

        return Ok(result.Value.Select(ser=>ser.ToResponseDto()));
    }

    /// <summary>
    /// Rename a service in the restaurant menu
    /// </summary>
    /// <param name="serviceName">For multi-word service names, use "-" as separators in the URL (e.g., "happy-hour", "lunch-special"). Service names are case-sensitive</param>
    /// <param name="dto"></param>
    /// <param name="restaurantId"></param>
    /// <returns>The newly added service</returns>
    /// <response code="200">Successfully updated the new service</response>
    /// <response code="400">Request body is invalid</response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// Insufficient permission
    /// - NOT_OWNER: Only the owner of a restaurant can update it
    /// </response>
    /// <response code="404">
    /// Not found
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// - SERVICE_NOT_FOUND: The supplied service name does not exist in this restaurant
    /// </response>
    /// <response code="409">
    /// Conflict with existing categories
    /// - SERVICE_ALREADY_EXISTS: The new service name conflicts with an existing category
    /// </response>
    [HttpPut("{serviceName}")]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(RestaurantServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Service>> RenameRestaurantService(Guid restaurantId, string serviceName, RenameRestaurantServiceDto body)
    {
        var result = await restaurantServicesService.RenameRestaurantService(restaurantId, serviceName, body.NewName);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantServiceError.ServiceConflictCode => Conflict(result.Error),
                RestaurantServiceError.ServiceNotFoundCode => NotFound(result.Error),

                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
        return Ok(result.Value.ToResponseDto());
    }

    /// <summary>
    /// Delete a service from a restaurant
    /// </summary>
    /// <param name="restaurantId"></param>
    /// <param name="serviceName">For multi-word service names, use "-" as separators in the URL (e.g., "happy-hour", "lunch-special"). Service names are case-sensitive</param>
    /// <returns>The service that has been deleted</returns>
    /// <response code="200">Successfully deleted the new category</response>
    /// <response code="400">Request body is invalid</response>
    /// <response code="401">
    /// User is not authenticated
    /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
    /// </response>
    /// <response code="403">
    /// Insufficient permission
    /// - NOT_OWNER: Only the owner of a restaurant can perform this action
    /// </response>
    /// <response code="404">
    /// Not found
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// - SERVICE_NOT_FOUND: The supplied service name does not exist in this restaurant
    /// </response>
    [HttpDelete("{serviceName}")]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(RestaurantServiceResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Service>> DeleteRestaurantService(Guid restaurantId, string serviceName)
    {
        var result = await restaurantServicesService.DeleteRestaurantService(restaurantId, serviceName);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantServiceError.ServiceNotFoundCode => NotFound(result.Error),

                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
        return Ok(result.Value.ToResponseDto());
    }
}