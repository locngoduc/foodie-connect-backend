using System.Security.Claims;
using foodie_connect_backend.Modules.DishReviews.Dtos;
using foodie_connect_backend.Modules.DishReviews.Mapper;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace foodie_connect_backend.Modules.DishReviews
{
   [Route("v1/dishes/{dishId:guid}/reviews")]
   [ApiController]
   public class DishReviewsController(DishReviewsService dishReviewsService) : ControllerBase
   {
       /// <summary>
       /// Add a review to a specific dish
       /// </summary>
       /// <param name="dishId">The unique identifier of the dish</param>
       /// <param name="dto">The review details including rating and comment</param>
       /// <returns>The created review details</returns>
       /// <response code="200">Successfully added review to the dish</response>
       /// <response code="400">Invalid request body</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - EMAIL_NOT_VERIFIED: Only users with verified email can perform this action
       /// </response>
       /// <response code="404">
       /// Dish not found
       /// - DISH_NOT_FOUND: This dish does not exist
       /// </response>
       /// <response code="409">
       /// Review already exists
       /// - ALREADY_REVIEWED: User has already reviewed this dish
       /// </response>
       [HttpPost]
       [Authorize(Policy = "EmailVerified")]
       [ProducesResponseType(typeof(DishReviewResponseDto), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       [ProducesResponseType(StatusCodes.Status409Conflict)]
       public async Task<ActionResult<DishReviewResponseDto>> AddReview(Guid dishId, CreateDishReviewDto dto)
       {
           var identity = HttpContext.User.Identity as ClaimsIdentity;
           var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
          
           var result = await dishReviewsService.AddReview(dishId, dto, userId);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.DishNotFoundCode => NotFound(result.Error),
                   DishError.AlreadyReviewedCode => Conflict(result.Error),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };


           return Ok(result.Value.ToResponseDto());
       }






       /// <summary>
       /// Get reviews of a specific dish
       /// </summary>
       /// <param name="dishId"></param>
       /// <returns></returns>
       /// <response code="200">The reviews of the specified dish</response>
       /// <response code="400">Dish ID is not in a valid format</response>
       [HttpGet]
       [ProducesResponseType(typeof(DishReviewsResponseDto), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       public async Task<ActionResult<DishReviewsResponseDto>> GetReviews(Guid dishId)
       {
           var identity = HttpContext.User.Identity as ClaimsIdentity;
           var userId = identity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
          
           var result = await dishReviewsService.GetReviews(dishId);
           var response = new DishReviewsResponseDto();


           if (userId is not null)
           {
               var userReview = result.Value.FirstOrDefault(x => x.UserId == userId);
               response.MyReview = userReview?.ToResponseDto();
               response.OtherReviews = result.Value
                   .Where(dishReview => dishReview.UserId != userId)
                   .Select(dishReview => dishReview.ToResponseDto())
                   .ToList();
           }
           else response.OtherReviews = result.Value.Select(dishReview => dishReview.ToResponseDto()).ToList();
          
           return Ok(response);
       }






       /// <summary>
       /// Updates a dish review
       /// </summary>
       /// <param name="reviewId"></param>
       /// <param name="dto"></param>
       /// <param name="dishId"></param>
       /// <returns></returns>
       /// <response code="200">Successfully updated the dish review</response>
       /// <response code="400">Invalid request body</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - EMAIL_NOT_VERIFIED: Only users with verified email can perform this action
       /// - NOT_AUTHORIZED: Only the dish reviewer can edit their reviews
       /// </response>
       /// <response code="404">
       /// Review not found
       /// - REVIEW_NOT_FOUND: This dish review does not exist
       /// </response>
       [HttpPut("{reviewId:guid}")]
       [Authorize(Policy = "EmailVerified")]
       [ProducesResponseType(typeof(DishReviewResponseDto), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<ActionResult<DishReviewResponseDto>> UpdateReview(Guid reviewId, UpdateDishReviewDto dto, Guid dishId)
       {
           var identity = HttpContext.User.Identity as ClaimsIdentity;
           var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
          
           var result = await dishReviewsService.UpdateReview(userId, reviewId, dto);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.ReviewNotFoundCode => NotFound(result.Error),
                   AuthError.NotAuthorizedCode => StatusCode(StatusCodes.Status403Forbidden, result.Error),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };
          
           return Ok(result.Value.ToResponseDto());
       }
      
      
      
       /// <summary>
       /// Delete a dish review
       /// </summary>
       /// <param name="reviewId"></param>
       /// <param name="dto"></param>
       /// <param name="dishId"></param>
       /// <returns></returns>
       /// <response code="200">Successfully deleted the dish review</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - EMAIL_NOT_VERIFIED: Only users with verified email can perform this action
       /// - NOT_AUTHORIZED: Only the dish reviewer can edit their reviews
       /// </response>
       /// <response code="404">
       /// Review not found
       /// - REVIEW_NOT_FOUND: This dish review does not exist
       /// </response>
       [HttpDelete("{reviewId:guid}")]
       [Authorize(Policy = "EmailVerified")]
       [ProducesResponseType(typeof(DishReviewResponseDto), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<ActionResult<DishReviewResponseDto>> UpdateReview(Guid reviewId, Guid dishId)
       {
           var identity = HttpContext.User.Identity as ClaimsIdentity;
           var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
          
           var result = await dishReviewsService.DeleteReview(userId, reviewId);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.ReviewNotFoundCode => NotFound(result.Error),
                   AuthError.NotAuthorizedCode => StatusCode(StatusCodes.Status403Forbidden, result.Error),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };
          
           return Ok(result.Value.ToResponseDto());
       }
   }
}

