using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Identity.Messages.Commands;
using BankingAppDDD.Identity.Services;
using BankingAppDDD.Infrastructures.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace BankingAppDDD.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IKeycloakAuthService _identityService;
        public AuthController(IKeycloakAuthService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
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


