using BankingAppDDD.UserManagement.Application.Users.Commands;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Domains.Users.Models;
using BankingAppDDD.Infrastructures.ActionResults;
using BankingAppDDD.UserManagement.Application.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Application.Users.Queries;
using BankingAppDDD.UserManagement.Core.Users.Models;


namespace BankingAppDDD.UserManagement.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/user")]
    public class UserController : ControllerBase
    {
        readonly IMediator _mediator;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updateuser")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateUser([FromBody] UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("userdetails")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<UserDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUser()
        {
            var customers = await _mediator.Send(new GetUserQuery());
            return Ok(customers);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var customer = await _mediator.Send(new GetUserQueryById(id));
            return Ok(customer);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [HttpGet("getuserprofile/{userid}")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(UserProfileDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetUserProfile(Guid userid)
        {
            var result = await _mediator.Send(new GetUserProfileQueryById(userid));
            return Ok(result);
        }
    }
}
