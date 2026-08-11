using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Identity.Messages.Commands;
using BankingAppDDD.Infrastructures.ActionResults;
using BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper;
using Microsoft.AspNetCore.Mvc;

namespace KeyCloakApiAccessManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakService _keycloakAdmin;

        public AuthController(IKeycloakService keycloakAdmin)
        {
            _keycloakAdmin = keycloakAdmin;
        }

        [HttpPost("login")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(JsonWebToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SignIn([FromBody] SignIn command)
        {
            var result = await _keycloakAdmin.GetUserTokenAsync(command.Username, command.Password);
            return Ok(result);
        }

        /*[HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] string refreshToken)
        {
            await _authService.RevokeTokenAsync(refreshToken);
            return NoContent();
        }


        [HttpPatch("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await _authService.ChangePasswordAsync(_currentUser.UserId!, dto);
            return NoContent();
        }*/

    }
}
