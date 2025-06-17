using System.Security.Claims;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Users.Dtos;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace foodie_connect_backend.Modules.Users
{
    [Route("v1/users")]
    [ApiController]
    [Produces("application/json")]
    public class UsersController(UsersService usersService) : ControllerBase
    {
        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="user">User creation data transfer object</param>
        /// <returns>The newly created user account</returns>
        /// <response code="201">Returns the newly created user account</response>
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
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> CreateUser(CreateUserDto user)
        {
            var result = await usersService.CreateUser(user);
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

            var responseDto = new UserResponseDto()
            {
                Id = result.Value.Id,
                UserName = result.Value.UserName!,
                DisplayName = result.Value.DisplayName,
            };

            return CreatedAtAction(nameof(GetUser), new { id = result.Value.Id }, responseDto);
        }



        /// <summary>
        /// Query basic information about a USER account
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Information about the USER account without sensitive information</returns>
        /// <response code="200">Returns the USER account information</response>
        /// <response code="404">
        /// Cannot find the queried user
        /// - USER_NOT_FOUND: User does not exist or is of different type (USER or HEAD)
        /// </response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var result = await usersService.GetUserById(id);
            if (result.IsFailure) return NotFound(result.Error);

            var responseDto = new UserResponseDto()
            {
                Id = result.Value.Id,
                UserName = result.Value.UserName!,
                DisplayName = result.Value.DisplayName,
                Avatar = result.Value.AvatarId,
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
                return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

            var result = await usersService.UploadAvatar(id, file);
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
        /// Change a USER account password
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
            if (userId == null || userId != id) return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

            var result = await usersService.ChangePassword(id, changePasswordDto);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    UserError.UserNotFoundCode => NotFound(result.Error),
                    UserError.PasswordMismatchCode => StatusCode(StatusCodes.Status403Forbidden, result.Error),
                    UserError.PasswordNotValidCode => BadRequest(result.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
                };
            }
            
            return Ok(new GenericResponse { Message = "Password changed successfully" });
        }



        /// <summary>
        /// Upgrade a USER to a HEAD account. This action is not reversible. This will destroy the user's current session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="200">Successfully upgraded the user account. REQUIRES USER TO RE-LOGIN</response>
        /// <response code="401">
        /// User is not authenticated
        /// - NOT_AUTHENTICATED: Only authenticated users can perform this action
        /// </response>
        /// <response code="403">
        /// User is not authorized or is already a HEAD
        /// - NOT_AUTHORIZED: Users can only upgrade their own accounts
        /// </response>
        [HttpPatch("{id}/type")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<GenericResponse>> UpgradeToHead(string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null || userId != id) return StatusCode(StatusCodes.Status403Forbidden, AuthError.NotAuthorized());

            var upgradeResult = await usersService.UpgradeToHead(userId);
            if (upgradeResult.IsFailure)
                return upgradeResult.Error.Code switch
                {
                    UserError.UserNotFoundCode => NotFound(upgradeResult.Error),
                    _ => StatusCode(StatusCodes.Status500InternalServerError, upgradeResult.Error)
                };

            return Ok(new GenericResponse { Message = "Upgraded user to head successfully" });
        }
    }

    [Route("v2/users")]
    [ApiController]
    [Produces("application/json")]
    public class UserControllerV2(UsersService usersService) : ControllerBase
    {
        /// <summary>
        /// Creates a new user account
        /// </summary>
        /// <param name="user">User creation data transfer object</param>
        /// <returns>The newly created user account</returns>
        /// <response code="201">Returns the newly created user account</response>
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
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<User>> CreateUser(CreateUserDto user)
        {
            var result = await usersService.CreateUser_V2(user);
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

            var responseDto = new UserResponseDto()
            {
                Id = result.Value.Id,
                UserName = result.Value.UserName!,
                DisplayName = result.Value.DisplayName,
            };

            return CreatedAtAction(nameof(GetUser), new { id = result.Value.Id }, responseDto);
        }
        
        /// <summary>
        /// Query basic information about a USER account
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Information about the USER account without sensitive information</returns>
        /// <response code="200">Returns the USER account information</response>
        /// <response code="404">
        /// Cannot find the queried user
        /// - USER_NOT_FOUND: User does not exist or is of different type (USER or HEAD)
        /// </response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> GetUser(string id)
        {
            var result = await usersService.GetUserById(id);
            if (result.IsFailure) return NotFound(result.Error);

            var responseDto = new UserResponseDto()
            {
                Id = result.Value.Id,
                UserName = result.Value.UserName!,
                DisplayName = result.Value.DisplayName,
                Avatar = result.Value.AvatarId,
            };
            return Ok(responseDto);
        }
    }
}
