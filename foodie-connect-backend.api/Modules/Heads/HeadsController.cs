using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Heads.Dtos;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Heads
{
    [Route("v1/heads")]
    [ApiController]
    [Produces("application/json")]
    public class HeadsController(HeadsService headsService) : ControllerBase
    {
        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="head">User creation data transfer object</param>
        /// <returns>The newly created user account</returns>
        /// <response code="201">Returns the newly created head account</response>
        /// <response code="400">
        /// Request body does not meet specified requirements
        /// - PASSWORD_NOT_VALID: Password does not meet requirements: at least 1x uppercase letter, 1x number, and 1x special character 
        /// </response>
        /// <response code="409">
        /// Conflict with existing data
        /// - USERNAME_ALREADY_EXISTS: The requested username is already taken
        /// - EMAIL_ALREADY_EXISTS: The requested email is already registered
        /// </response>
        /// <response code="500">
        /// Internal server error
        /// - INTERNAL_ERROR: An unexpected internal error occurred
        /// - UNEXPECTED_ERROR: An unexpected error occurred
        /// </response>
        [HttpPost]
        [ProducesResponseType(typeof(HeadResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> CreateHead(CreateHeadDto head)
        {
            var result = await headsService.CreateHead(head);
            
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserError.DuplicateUsernameCode => Conflict(result.Error),
                    UserError.DuplicateEmailCode => Conflict(result.Error),
                    UserError.PasswordNotValidCode => BadRequest(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };
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
        /// Query basic information about a HEAD account
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Information about the USER account without sensitive information</returns>
        /// <response code="200">Returns the USER account information</response>
        /// <response code="404">
        /// Cannot find the queried user
        /// - USER_NOT_FOUND: User does not exist or is of different type (USER or HEAD)
        /// </response>
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
        /// <response code="400">
        /// Image does not meet requirements
        /// - TYPE_NOT_ALLOWED: The image's extension is not allowed, allowed extensions: .png, .jpg, .jpeg, .webp
        /// - EXCEED_MAX_SIZE: Maximum file size is 5MB
        /// </response>
        /// <response code="401">
        /// User is not authenticated
        /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
        /// </response>
        /// <response code="403">
        /// User is not authorized
        /// - NOT_AUTHORIZED: Users can only change their own accounts' avatars
        /// </response>
        [HttpPatch("{id}/avatar")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize]
        public async Task<ActionResult<GenericResponse>> UploadAvatar(string id, IFormFile file)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || userId != id)
                return Unauthorized(AuthError.NotAuthorized());

            var result = await headsService.UploadAvatar(id, file);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserError.UserNotFoundCode => NotFound(result.Error),
                    UploadError.ExceedMaxSizeCode => BadRequest(result.Error),
                    UploadError.TypeNotAllowedCode => BadRequest(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };
            }

            return Ok(new GenericResponse() { Message = "Avatar updated successfully" });
        }
        
        
        
        /// <summary>
        /// Change a HEAD account password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="changePasswordDto"></param>
        /// <returns>Password change result</returns>
        /// <response code="200">Password changed successfully</response>
        /// <response code="400">
        /// Password does not meet requirements
        /// - PASSWORD_NOT_VALID: Password does not meet requirements: at least 1x uppercase letter, 1x number, and 1x special character 
        /// </response>
        /// <response code="401">
        /// User is not authenticated
        /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
        /// </response>
        /// <response code="403">
        /// User is not authorized
        /// - NOT_AUTHORIZED: Users can only change their own accounts' passwords
        /// - PASSWORD_MISMATCH: Old password is incorrect
        /// </response>
        [HttpPatch("{id}/password")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize]
        public async Task<ActionResult<GenericResponse>> ChangePassword(string id, ChangePasswordDto changePasswordDto)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId == null || userId != id) return Unauthorized(AuthError.NotAuthorized());

            var result = await headsService.ChangePassword(id, changePasswordDto);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserError.UserNotFoundCode => NotFound(result.Error),
                    UserError.PasswordMismatchCode => Unauthorized(result.Error),
                    UserError.PasswordNotValidCode => BadRequest(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };
            }
            
            return Ok(new GenericResponse { Message = "Password changed successfully" });
        }
    }
}
