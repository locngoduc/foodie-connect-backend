using System.Security.Claims;
using foodie_connect_backend.Sessions.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace foodie_connect_backend.Sessions
{
    [Route("v1/sessions")]
    [ApiController]
    [Produces("application/json")]
    public class SessionsController(SessionsService sessionsService) : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult<LoginResponse>> Login(LoginDto loginDto)
        {
            var result = await sessionsService.LoginUser(loginDto.UserName, loginDto.Password);
            if (result.IsFailure) return Unauthorized(new LoginResponse { Message = result.Error.Message });
            return new LoginResponse { Message = "Successfully logged in" };
        }



        [HttpGet]
        [Authorize]
        public async Task<ActionResult<bool>> GetSessionInfo()
        {
            var identity = User.Identity as ClaimsIdentity;
            var userType = identity?.Claims.FirstOrDefault(c => c.Type == "UserType");
            Console.WriteLine(userType?.Value);

            return true;
        }
    }
}
