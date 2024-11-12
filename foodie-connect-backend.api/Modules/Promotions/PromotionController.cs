using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Promotions.Dtos;
using foodie_connect_backend.Modules.Promotions.Mapper;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using GoogleApi.Entities.Maps.Geolocation.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Promotions
{
    [Route("v1/restaurants/{restaurantId:guid}/promotions")]
    [ApiController]
    public class PromotionController(PromotionsService promotionsService) : ControllerBase
    {
        /// <summary>
        /// Add a promotion to a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="201">Successfully added the new promotion</response>
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
        /// Restaurant not found
        /// - PROMOTION_DISH_NOT_FOUND: No dish with the specified ID was found in the restaurant for the promotion
        /// </response>
        [HttpPost]
        [Authorize(Policy = "RestaurantOwner")]
        [ProducesResponseType(typeof(PromotionResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PromotionResponseDto>> AddPromotion(Guid restaurantId, CreatePromotionDto dto)
        {
            var result = await promotionsService.CreatePromotion(restaurantId, dto);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    PromotionError.PromotionDishNotFoundCode => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return CreatedAtAction(nameof(GetPromotion), new { promotionId = result.Value.Id }, result.Value.ToResponseDto());
        }
        
        
        
        /// <summary>
        /// Retrieve promotions of a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        /// <response code="200">Successfully added the new promotion</response>
        [HttpGet]
        [ProducesResponseType(typeof(IList<PromotionResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PromotionResponseDto>> GetPromotions(Guid restaurantId)
        {
            var result = await promotionsService.GetPromotions(restaurantId);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value.Select(x => x.ToResponseDto()));
        }

        
        
        /// <summary>
        /// Retrieve a promotion
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        /// <response code="200">The queried promotion</response>
        /// <response code="404">Promotion not found</response>
        [HttpGet("{promotionId:guid}")]
        [ProducesResponseType(typeof(PromotionResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PromotionResponseDto>> GetPromotion(Guid promotionId, Guid restaurantId)
        {
            var result = await promotionsService.GetPromotion(promotionId, restaurantId);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    PromotionError.PromotionNotFoundCode => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value.ToResponseDto());
        }
        
        
        
        /// <summary>
        /// Delete a promotion
        /// </summary>
        /// <param name="promotionId"></param>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        /// <response code="200">Promotion deleted</response>
        /// <response code="401">
        /// User is not authenticated
        /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
        /// </response>
        /// <response code="403">
        /// Insufficient permission
        /// - NOT_OWNER: Only the owner of a restaurant can perform this action
        /// </response>
        /// <response code="404">Promotion not found</response>
        [HttpDelete("{promotionId:guid}")]
        [Authorize(Policy = "RestaurantOwner")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<GenericResponse>> DeletePromotion(Guid promotionId, Guid restaurantId)
        {
            var result = await promotionsService.DeletePromotion(promotionId, restaurantId);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    PromotionError.PromotionNotFoundCode => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(new GenericResponse { Message = "Promotion deleted"});
        }
    }
}
