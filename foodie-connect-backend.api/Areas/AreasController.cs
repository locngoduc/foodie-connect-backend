using System.Security.Claims;
using foodie_connect_backend.Area.Dtos;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Areas;

[Route("v1/areas")]
[ApiController]
public class AreasController(
    AreasService areasService
    ): ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Data.Area>> CreateArea(CreateAreaDto createAreaDto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var result = await areasService.CreateArea(createAreaDto, userId);
        if (result.IsFailure)
        {
            if (result.IsFailure)
            {
                if (result.Error.Code == AppError.ConflictErrorCode)
                {
                    return Conflict(result.Error);
                }

                // if (result.Error.Code == AppError.PermissionDeniedErrorCode)
                //     return Forbid();
                return BadRequest(result.Error);
            }
        }

        return Ok(result.Value);
    }
}