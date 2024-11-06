using System.Security.Claims;
using foodie_connect_backend.Modules.Sessions.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Dtos;
using foodie_connect_backend.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Sessions
{
    [Route("v1/session")]
    [ApiController]
    [Produces("application/json")]
    public class SessionsController(SessionsService sessionsService) : ControllerBase
    {
        /// <summary>
        /// Login to the application either as a HEAD(1) or a USER(0)
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns>The login result</returns>
        /// <response code="200">Successfully logged in</response>
        /// <response code="400">Request body is malformed</response>
        /// <response code="401">Invalid credentials</response>
        [HttpPost]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<GenericResponse>> Login(LoginDto loginDto)
        {
            Result<bool> result;
            
            switch (loginDto.LoginType)
            {
                case LoginType.User:
                    result = await sessionsService.LoginUser(loginDto.UserName, loginDto.Password);
                    break;
                case LoginType.Head:
                    result = await sessionsService.LoginHead(loginDto.UserName, loginDto.Password);
                    break;
                default:
                    return BadRequest(new { Message = "Login type not supported" });
            }
            
            if (result.IsFailure) return Unauthorized(new GenericResponse { Message = result.Error.Message });
            return new GenericResponse { Message = "Successfully logged in" };
        }



        /// <summary>
        /// Query the logged-in user session details
        /// </summary>
        /// <returns>Logged-in user details</returns>
        /// <response code="200">User session is valid</response>
        /// <response code="401">User session is invalid</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(SessionInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SessionInfo>> GetSessionInfo()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            var userId = identity!.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var result = await sessionsService.GetUserSession(userId);
            if (result.IsFailure) return Unauthorized(new GenericResponse { Message = result.Error.Message });
            return Ok(result.Value);
        }



        /// <summary>
        /// Logout of current session
        /// </summary>
        /// <returns>Logout</returns>
        /// <response code="200">Successfully logged out</response>
        /// <response code="401">Not logged in</response>
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<GenericResponse>> Logout()
        {
            var result = await sessionsService.Logout();
            if (result.IsFailure) return Unauthorized(new GenericResponse { Message = result.Error.Message });
            return Ok(new GenericResponse { Message = "Successfully logged out" });
        }
    }
}
