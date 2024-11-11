using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Promotions.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Promotions
{
    [Route("v1/restaurants/{restaurantId}/promotions")]
    [ApiController]
    public class PromotionController(PromotionsService promotionsService) : ControllerBase
    {
        /// <summary>
        /// Add a promotion to a restaurant
        /// </summary>
        /// <param name="restaurantId"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<Promotion>> AddPromotion(Guid restaurantId, CreatePromotionDto dto)
        {
            return StatusCode(StatusCodes.Status501NotImplemented);
        }
    }
}
