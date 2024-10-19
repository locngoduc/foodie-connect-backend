using foodie_connect_backend.Data;
using foodie_connect_backend.Heads.Dtos;
using foodie_connect_backend.Shared.Classes;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Heads
{
    [Route("v1/heads")]
    [ApiController]
    [Produces("application/json")]
    public class HeadsController(HeadsService headsService) : ControllerBase
    {
        /// <summary>
        /// Create a HEAD account
        /// </summary>
        /// <param name="head"></param>
        /// <returns>The newly created HEAD account</returns>
        /// <response code="201">Returns the newly created HEAD account</response>
        /// <response code="400">Request body does not meet specified requirements</response>
        /// <response code="409">Username or Email is taken</response>
        [HttpPost]
        [ProducesResponseType(typeof(HeadResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> CreateHead(CreateHeadDto head)
        {
            var result = await headsService.CreateHead(head);
            
            if (result.IsFailure)
            {
                if (result.Error.Code == AppError.ConflictErrorCode)
                {
                    return Conflict(result.Error);
                }
                return BadRequest(result.Error);
            }

            var responseDto = new HeadResponseDto
            {
                Id = result.Value.Id,
                UserName = result.Value.UserName!,
                DisplayName = result.Value.DisplayName,
                PhoneNumber = result.Value.PhoneNumber!,
                Email = result.Value.Email!,
                EmailConfirmed = result.Value.EmailConfirmed,
            };
            
            return CreatedAtAction(nameof(GetHead), new { id = result.Value.Id }, responseDto);
        }

        
        
        /// <summary>
        /// Query basic information about a HEAD user
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Information about the HEAD user without sensitive informations</returns>
        /// <response code="200">Returns the HEAD user information</response>
        /// <response code="404">HEAD user not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(HeadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetHead(string id)
        {
            var result = await headsService.GetHeadById(id);
            if (result.IsFailure) return NotFound(result.Error);

            var responseDto = new HeadResponseDto
            {
                Id = result.Value.Id,
                UserName = result.Value.UserName!,
                DisplayName = result.Value.DisplayName,
            };
            return Ok(responseDto);
        }
    }
}
