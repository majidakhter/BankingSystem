using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Identity.Application.IAM.Commands;
using BankingAppDDD.Identity.Messages.Commands;
using BankingAppDDD.Infrastructures.ActionResults;
using BankingAppDDD.KeyCloakClientLibrary.KeyCloakRestHelper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingAppDDD.Identity.Controllers
{
    /// <summary>
    /// Login, session management, MFA, biometrics
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakService _identityService;
        private readonly IMediator _mediator;
        public AuthController(IKeycloakService identityService, IMediator mediator)
        {
            _identityService = identityService;
            _mediator = mediator;
        }

        [HttpPost("login")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(JsonWebToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SignIn([FromBody] SignIn command)
        {
            var result = await _identityService.GetUserTokenAsync(command.Username, command.Password);
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("openaccount")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> OpenAccount([FromForm] RegisterIdentityCommand command)
        {
            var result = await _mediator.Send(command);
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


