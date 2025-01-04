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
    [HttpPost]
    //[Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    [HttpGet]
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

    [HttpPut("{serviceName}")]
    //[Authorize(Policy = "RestaurantOwner")]
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

    [HttpDelete("{serviceName}")]
    //[Authorize(Policy = "RestaurantOwner")]
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