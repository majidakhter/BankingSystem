using BankingAppDDD.Common.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingAppDDD.Identity.Controllers
{

    /// <summary>
    /// Controller class just to validate the User Role Permission.
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/value")]
    public class ValueController : ControllerBase
    {

        private readonly ILogger<ValueController> _logger;

        /// <summary>
        /// Constructor of ValueController.
        /// </summary>
        public ValueController(ILogger<ValueController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns a message indicating the user is authorized as a Admin user.
        /// </summary>
        /// <returns>Ok result with a general user message.</returns>
        [HttpGet("get-admin")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetAdmin()
        {
            return Ok("You are admin.");
        }

        /// <summary>
        /// Returns a message indicating the user is authorized as a general user.
        /// </summary>
        /// <returns>Ok result with a general user message.</returns>
        [HttpGet("get-general")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public IActionResult GetGeneral()
        {
            return Ok("You are general.");
        }

        
    }
}
