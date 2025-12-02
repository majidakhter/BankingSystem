using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Identity.Messages.Commands;
using BankingAppDDD.Identity.Services;
using BankingAppDDD.Infrastructures.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace BankingAppDDD.Identity.Controllers
{
    
    [ApiController]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakAuthService _identityService;
        public AuthController(IKeycloakAuthService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(JsonWebToken), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> SignIn([FromBody] SignIn command)
        {
            var result = await _identityService.SignInAsync(command.Username, command.Password);
            return Ok(result);
        }


    }
}


