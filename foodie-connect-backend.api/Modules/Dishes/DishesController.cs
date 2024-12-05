using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Dishes.Dtos;
using foodie_connect_backend.Modules.Dishes.Hub;
using foodie_connect_backend.Modules.Dishes.Mapper;
using foodie_connect_backend.Modules.DishReviews.Dtos;
using foodie_connect_backend.Modules.DishReviews.Mapper;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace foodie_connect_backend.Modules.Dishes
{
   [Route("v1/dishes")]
   [ApiController]
   [Produces("application/json")]
   public class DishesController(DishesService dishesService, ActiveDishViewersService dishViewersService) : ControllerBase
   {
       /// <summary>
       /// Add a dish to a restaurant's menu
       /// </summary>
       /// <param name="dto"></param>
       /// <returns></returns>
       /// <response code="201">The added dish</response>
       /// <response code="400">Invalid request body</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - NOT_OWNER: Only the owner can perform this action
       /// </response>
       /// <response code="409">
       /// Conflict with existing data
       /// - NAME_ALREADY_EXISTS: Another dish already has this name
       /// </response>
       [HttpPost]
       [Authorize(Policy = "RestaurantOwner")]
       [ProducesResponseType(typeof(DishResponseDto), StatusCodes.Status201Created)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status409Conflict)]
       public async Task<ActionResult<Dish>> AddDish(CreateDishDto dto)
       {
           var result = await dishesService.AddDishToRestaurant(dto);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.NameAlreadyExistsCode => Conflict(result.Error),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };
          
           return CreatedAtAction(nameof(GetDishById), new { id = result.Value.Id }, result.Value.ToResponseDto());
       }






       /// <summary>
       /// Query dishes
       /// </summary>
       /// <param name="query"></param>
       /// <returns></returns>
       /// <response code="200">The dishes that satisfy the query</response>
       /// <response code="400">Missing required fields</response>
       /// <response code="404">
       /// Not found
       /// - RESTAURANT_NOT_FOUND: The provided restaurant ID does not match any existing restaurant
       /// </response>
       [HttpGet]
       [ProducesResponseType(typeof(IEnumerable<DishResponseDto>), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<ActionResult<List<DishResponseDto>>> GetDishes([FromQuery] GetDishesQuery query)
       {
           var result = await dishesService.QueryDishes(query);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.RestaurantNotFoundCode => NotFound(result.Error),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };


           // Calculate all scores in a single query
           var dishIds = result.Value.Select(d => d.Id);
           var scores = await dishesService.CalculateDishScoresAsync(dishIds);


           // Map the results
           var dishes = result.Value.Select(dish =>
                   dish.ToResponseDto(scores.GetValueOrDefault(dish.Id, new ScoreResponseDto())))
               .ToList();


           return Ok(dishes);
       }






       /// <summary>
       /// Get a specific dish
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
       /// <response code="200">The dishes that satisfy the query</response>
       /// <response code="400">Provided dish ID is incorrect</response>
       /// <response code="404">
       /// Not found
       /// - DISH_NOT_FOUND: The provided dish ID does not match any existing dish
       /// </response>
       [HttpGet("{id:Guid}")]
       [ProducesResponseType(typeof(DishResponseDto), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<ActionResult<DishResponseDto>> GetDishById(Guid id)
       {
           var result = await dishesService.GetDishById(id);
           if (result.IsFailure) return NotFound(result.Error);
  
           var score = await dishesService.CalculateDishScoreAsync(id);
           return Ok(result.Value.ToResponseDto(score));
       }
      
      
      
       /// <summary>
       /// Update dish details
       /// </summary>
       /// <param name="dishId"></param>
       /// <param name="dto"></param>
       /// <returns></returns>
       /// <response code="200">Successfully updated the dish</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - NOT_OWNER: Only the owner can perform this action
       /// </response>
       /// <response code="404">
       /// Dish not found
       /// - DISH_NOT_FOUND: This dish does not exist
       /// </response>
       /// <response code="409">
       /// Conflict with existing data
       /// - NAME_ALREADY_EXISTS: Another dish already has this name
       /// </response>
       [HttpPut("{dishId:guid}")]
       [Authorize(Policy = "DishOwner")]
       [ProducesResponseType(typeof(DishResponseDto), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       [ProducesResponseType(StatusCodes.Status409Conflict)]
       public async Task<ActionResult<DishResponseDto>> UpdateDish(Guid dishId, UpdateDishDto dto)
       {
           var result = await dishesService.UpdateDish(dishId, dto);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.DishNotFoundCode => NotFound(result.Value),
                   DishError.NameAlreadyExistsCode => Conflict(result.Error),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };
  
           var score = await dishesService.CalculateDishScoreAsync(dishId);
           return Ok(result.Value.ToResponseDto(score));
       }
      
      
      
       /// <summary>
       /// Delete a dish from a restaurant's menu
       /// </summary>
       /// <param name="dishId"></param>
       /// <returns></returns>
       /// <response code="200">Successfully deleted the dish</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - NOT_OWNER: Only the owner can perform this action
       /// </response>
       /// <response code="404">
       /// Dish not found
       /// - DISH_NOT_FOUND: This dish does not exist
       /// </response>
       [HttpDelete("{dishId:guid}")]
       [Authorize(Policy = "DishOwner")]
       [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<ActionResult<GenericResponse>> DeleteDish(Guid dishId)
       {
           var result = await dishesService.DeleteDish(dishId);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   DishError.DishNotFoundCode => NotFound(result.Value),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };
          
           return Ok(new GenericResponse { Message = "Dish deleted successfully" });
       }




      
       /// <summary>
       /// Set display image for a dish
       /// </summary>
       /// <param name="dishId"></param>
       /// <param name="file"></param>
       /// <returns></returns>
       /// <response code="200">Successfully set dish image</response>
       /// <response code="400">Invalid request body</response>
       /// <response code="401">
       /// User is not authenticated
       /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
       /// </response>
       /// <response code="403">
       /// Insufficient permission
       /// - NOT_OWNER: Only the owner can perform this action
       /// </response>
       /// <response code="404">
       /// Dish not found
       /// - DISH_NOT_FOUND: This dish does not exist
       /// </response>
       [HttpPut("{dishId:guid}/image")]
       [Authorize(Policy = "DishOwner")]
       [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
       [ProducesResponseType(StatusCodes.Status400BadRequest)]
       [ProducesResponseType(StatusCodes.Status401Unauthorized)]
       [ProducesResponseType(StatusCodes.Status403Forbidden)]
       [ProducesResponseType(StatusCodes.Status404NotFound)]
       public async Task<ActionResult<GenericResponse>> SetDishImage(Guid dishId, IFormFile file)
       {
           var result = await dishesService.SetDishImage(dishId, file);
           if (result.IsFailure)
               return result.Error.Code switch
               {
                   UploadError.TypeNotAllowedCode => BadRequest(result.Error),
                   UploadError.ExceedMaxSizeCode => BadRequest(result.Error),
                   DishError.DishNotFoundCode => NotFound(result.Value),
                   _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
               };
          
           return Ok(new GenericResponse { Message = "Dish image updated successfully" });
       }
      
      
      
       /// <summary>
       /// Get current viewer count for a dish
       /// </summary>
       /// <param name="dishId">The ID of the dish</param>
       /// <returns>Number of current viewers</returns>
       [HttpGet("{dishId:guid}/viewers/count")]
       [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
       public ActionResult<int> GetDishViewerCount(Guid dishId)
       {
           var viewerCount = dishViewersService.GetCurrentViewers(dishId);
           return Ok(viewerCount);
       }
   }
}

