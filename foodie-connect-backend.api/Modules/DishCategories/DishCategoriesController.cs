using foodie_connect_backend.Modules.DishCategories.Dtos;
using foodie_connect_backend.Modules.DishCategories.Mapper;
using foodie_connect_backend.Modules.Restaurants.Dtos;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.DishCategories
{
    [Route("v1/restaurants/{restaurantId}/categories/")]
    [ApiController]
    public class DishCategoriesController(DishCategoriesService dishCategoriesService) : ControllerBase
    {
            /// <summary>
    /// Get a list of categories in the restaurant
    /// </summary>
    /// <param name="id"></param>
    /// <returns>A list of categories</returns>
    /// <response code="200">Successfully queried the restaurant's categories</response>
    /// <response code="404">
    /// Restaurant not found
    /// - RESTAURANT_NOT_EXIST: The provided ID does not correspond to any restaurant
    /// </response>
    [HttpGet]
    [ProducesResponseType(typeof(DishCategoryResponseDto[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DishCategoryResponseDto[]>> GetCategories(Guid id)
    {
        var result = await dishCategoriesService.GetDishCategories(id);
        if (result.IsFailure) return NotFound(result.Error);
        
        return Ok(result.Value.Select(x => x.ToResponseDto()));
    }
    
    
    
    /// <summary>
    /// Adds a category to the restaurant menu
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <returns>The newly added category</returns>
    /// <response code="200">Successfully added the new category</response>
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
    /// Restaurant not found
    /// - RESTAURANT_NOT_EXIST: This restaurant does not exist
    /// </response>
    /// <response code="409">
    /// Conflict with existing categories
    /// - DISH_CATEGORY_ALREADY_EXIST: This category name already exists
    /// </response>
    [HttpPost]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(DishCategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DishCategoryResponseDto>> AddDishCategory(Guid id, CreateDishCategoryDto dto)
    {
        var result = await dishCategoriesService.AddDishCategory(id, dto.CategoryName);
        if (result.IsFailure)
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantError.DishCategoryAlreadyExistCode => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };

        return Ok(result.Value.ToResponseDto());
    }


    
    /// <summary>
    /// Delete a category from the restaurant menu
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoryName"></param>
    /// <returns>The newly added category</returns>
    /// <response code="200">Successfully added the new category</response>
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
    /// - DISH_CATEGORY_NOT_EXIST: The supplied category name does not exist in this restaurant
    /// </response>
    [HttpDelete("{categoryName}")]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(DishCategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DishCategoryResponseDto>> DeleteDishCategory(Guid id, string categoryName)
    {
        var result = await dishCategoriesService.DeleteDishCategory(id, categoryName);
        if (result.IsFailure)
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantError.DishCategoryNotExistCode => NotFound(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };

        return Ok(result.Value.ToResponseDto());
    }

    /// <summary>
    /// Rename a category in the restaurant menu
    /// </summary>
    /// <param name="id"></param>
    /// <param name="categoryName"></param>
    /// <param name="dto"></param>
    /// <returns>The newly added category</returns>
    /// <response code="200">Successfully added the new category</response>
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
    /// - DISH_CATEGORY_NOT_EXIST: The supplied category name does not exist in this restaurant
    /// </response>
    /// <response code="409">
    /// Conflict with existing categories
    /// - DISH_CATEGORY_ALREADY_EXIST: The new category name conflicts with an existing category
    /// </response>
    [HttpPut("{categoryName}")]
    [Authorize(Policy = "RestaurantOwner")]
    [ProducesResponseType(typeof(DishCategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<DishCategoryResponseDto>> RenameDishCategory(Guid id, string categoryName, RenameDishCategoryDto dto)
    {
        var result = await dishCategoriesService.RenameDishCategory(id, categoryName, dto.NewName);
        if (result.IsFailure)
            return result.Error.Code switch
            {
                RestaurantError.RestaurantNotExistCode => NotFound(result.Error),
                RestaurantError.DishCategoryNotExistCode => NotFound(result.Error),
                RestaurantError.DishCategoryAlreadyExistCode => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };

        return Ok(result.Value.ToResponseDto());
    }
    }
}
