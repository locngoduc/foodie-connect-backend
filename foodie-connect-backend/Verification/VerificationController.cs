using System.Security.Claims;
using foodie_connect_backend.Sessions.Dtos;
using foodie_connect_backend.Verification.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Verification
{
    [Route("v1/verification")]
    [ApiController]
    [Produces("application/json")]
    public class VerificationController(VerificationService verificationService) : ControllerBase
    {
        /// <summary>
        /// Verify logged-in user email
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <response code="400">Invalid request body</response>
        /// <response code="401">Not logged in or invalid token</response>
        [HttpPost("email")]
        [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        public async Task<ActionResult<GenericResponse>> ConfirmEmail(ConfirmEmailDto dto)
        {
            var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
            
            var result = await verificationService.ConfirmEmail(userId, dto.EmailToken);
            if (result.IsFailure)
            {
                return result.Error.Code switch
                {
                    "RecordNotFound" => NotFound(result.Error),
                    "BadToken" => Unauthorized(result.Error),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
            
            return Ok(new GenericResponse { Message = "Email verification successful." });
        }
    }
}
