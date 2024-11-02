using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Heads.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
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
                Avatar = result.Value.AvatarId
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



        /// <summary>
        /// Change the logged-in user avatar
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <response code="200">Successfully uploaded avatar</response>
        /// <response code="400">Invalid body. Allowed extensions are png, jpg, jpeg, and webp. Image must be less than 5MB</response>
        /// <response code="401">Not logged in or not authorized to change another user avatar</response>
        /// <response code="404">Invalid id provided</response>
        [HttpPatch("{id}/avatar")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<GenericResponse>> UploadAvatar(string id, IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || userId != id) 
                return Unauthorized(AppError.InvalidCredential("You are not authorized to perform this operation"));
            
            var result = await headsService.UploadAvatar(id, file);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "RecordNotFound" => NotFound(result.Error),
                    "ValidationError" => BadRequest(result.Error),
                    _ => StatusCode(500, result.Error),
                };
            }

            return Ok(new GenericResponse() { Message = "Avatar updated successfully" });
        }
        
        
        
        /// <summary>
        /// Change a HEAD user password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="changePasswordDto"></param>
        /// <returns>Password change result</returns>
        /// <response code="200">Password changed successfully</response>
        /// <response code="400">Invalid request body or new password does not meet requirements</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">No HEAD user found</response>
        [HttpPatch("{id}/password")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize]
        public async Task<ActionResult<GenericResponse>> ChangePassword(string id, ChangePasswordDto changePasswordDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || userId != id) return Unauthorized(AppError.InvalidCredential("You are not authorized to change this user's password"));
            
            var result = await headsService.ChangePassword(id, changePasswordDto);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "RecordNotFound" => NotFound(result.Error),
                    "InvalidCredential" => Unauthorized(result.Error),
                    _ => BadRequest(result.Error)
                };
            }
            return Ok(new GenericResponse { Message = "Password changed successfully" });
        }
    }
}
