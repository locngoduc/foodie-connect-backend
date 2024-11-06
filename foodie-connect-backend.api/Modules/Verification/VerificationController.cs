using System.Security.Claims;
using foodie_connect_backend.Modules.Verification.Dtos;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Modules.Verification;

[Route("v1/verification")]
[ApiController]
[Produces("application/json")]
public class VerificationController(VerificationService verificationService) : ControllerBase
{
    /// <summary>
    ///     Re-send verification email to the logged-in user
    /// </summary>
    /// <returns></returns>
    /// <response code="200">Successfully sent verification email</response>
    /// <response code="400">User email is already verified</response>
    /// <response code="401">Not logged-in</response>
    /// <response code="500">Internal error</response>
    [HttpGet("email")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<GenericResponse>> ResendConfirmationEmail()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var result = await verificationService.SendConfirmationEmail(userId!);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                VerificationError.UserNotFoundCode => NotFound(result.Error),
                VerificationError.EmailAlreadyConfirmedCode => BadRequest(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error),
            };
        } 
            
        return Ok(new GenericResponse { Message = "Verification email sent successfully" });
    }

    /// <summary>
    ///     Verify logged-in user email
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <response code="400">Invalid request body or invalid token</response>
    /// <response code="401">Not logged in</response>
    /// <response code="500">Internal error</response>
    [HttpPost("email")]
    [ProducesResponseType(typeof(GenericResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize]
    public async Task<ActionResult<GenericResponse>> ConfirmEmail(ConfirmEmailDto dto)
    {
        var userId = User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

        var result = await verificationService.ConfirmEmail(userId, dto.EmailToken);
        if (result.IsFailure)
        {
            return result.Error.Code switch
            {
                VerificationError.UserNotFoundCode => NotFound(result.Error),
                VerificationError.EmailVerificationTokenInvalidCode => BadRequest(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error),
            };
        } 

        return Ok(new GenericResponse { Message = "Email verification successful." });
    }
}