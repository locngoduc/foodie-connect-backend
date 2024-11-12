using System.Security.Claims;
using foodie_connect_backend.Modules.RestaurantReviews.Dtos;
using foodie_connect_backend.Modules.RestaurantReviews.Mapper;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.RestaurantReviews
{
    [Route("v1/restaurants/{restaurantId:guid}/reviews")]
    [ApiController]
    public class RestaurantReviewsController(RestaurantReviewsService restaurantReviewsService) : ControllerBase
    {
        /// <summary>
        /// Add a review to a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="201">Successfully added the new review</response>
        /// <response code="400">Request body is invalid</response>
        /// <response code="401">
        /// User is not authenticated
        /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
        /// </response>
        /// <response code="403">
        /// Insufficient permission
        /// - EMAIL_NOT_VERIFIED: Only users with verified email can perform this action
        /// </response>
        /// <response code="409">
        /// Restaurant not found
        /// - ALREADY_REVIEWED: User has already reviewed this restaurant
        /// </response>
        [HttpPost]
        [Authorize(Policy = "EmailVerified")]
        [ProducesResponseType(typeof(RestaurantReviewResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantReviewResponseDto>> AddReview(Guid restaurantId, CreateRestaurantReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await restaurantReviewsService.AddReview(restaurantId, userId!, dto);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    RestaurantError.AlreadyReviewedCode => Conflict(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return StatusCode(StatusCodes.Status201Created, result.Value.ToResponseDto());
        }
        
        
        
        /// <summary>
        /// Retrieve reviews of a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <returns></returns>
        /// <response code="200">Successfully retrieved the reviews</response>
        /// <response code="400">Restaurant not found</response>
        [HttpGet]
        [ProducesResponseType(typeof(RestaurantReviewsResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantReviewsResponseDto>> GetReviews(Guid restaurantId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await restaurantReviewsService.GetReviews(restaurantId, userId);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value);
        }
        
        
        
        /// <summary>
        /// Update a review of a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="reviewId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <response code="200">Successfully updated the review</response>
        /// <response code="400">Request body is invalid</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">Only the reviewer than update their review</response>
        /// <response code="404">Review not found</response>
        [HttpPut("{reviewId:guid}")]
        [Authorize(Policy = "EmailVerified")]
        [ProducesResponseType(typeof(RestaurantReviewResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantReviewResponseDto>> UpdateReview(Guid restaurantId, Guid reviewId, CreateRestaurantReviewDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await restaurantReviewsService.UpdateReview(restaurantId, userId!, reviewId, dto);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    RestaurantError.ReviewNotExistCode => NotFound(result.Error),
                    AuthError.NotAuthorizedCode => StatusCode(StatusCodes.Status403Forbidden, result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(result.Value.ToResponseDto());
        }
        
        
        
        /// <summary>
        /// Delete a review of a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        /// <response code="200">Successfully updated the review</response>
        /// <response code="401">User is not authenticated</response>
        /// <response code="403">Only the reviewer than delete their review</response>
        /// <response code="404">Review not found</response>
        [HttpDelete("{reviewId:guid}")]
        [Authorize(Policy = "EmailVerified")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantReviewResponseDto>> DeleteReview(Guid restaurantId, Guid reviewId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await restaurantReviewsService.DeleteReview(restaurantId, reviewId, userId!);
            if (result.IsFailure)
                return result.Error.Code switch
                {
                    RestaurantError.ReviewNotExistCode => NotFound(result.Error),
                    AuthError.NotAuthorizedCode => StatusCode(StatusCodes.Status403Forbidden, result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };

            return Ok(new GenericResponse { Message = "Review deleted successfully" });
        }
    }
}
