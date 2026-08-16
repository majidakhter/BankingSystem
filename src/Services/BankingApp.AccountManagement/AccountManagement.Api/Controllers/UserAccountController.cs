using BankingApp.AccountManagement.Application.Accounts.Queries;
using BankingApp.AccountManagement.Application.CustomerAccounts.Models;
using BankingApp.AccountManagement.Application.CustomerAccounts.Queries;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Domains.Accounts.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    /// <summary>
    /// Profile, KYC data, account preferences
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/user")]

    public class UserAccountController : ControllerBase
    {
        readonly IMediator _mediator;
        public UserAccountController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("getuseraccountcount/{userId}")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(CustomerAccountDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserAccountCountById(Guid userId)
        {
            var user = await _mediator.Send(new GetCustomerAccountCountById(userId));
            return Ok(user);
        }

        [HttpGet("getaccountdetails/{userId}")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(UserAccountDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAccountDetailsById(Guid userId)
        {
            var useraccountdetails = await _mediator.Send(new GetAccountDetailsByIdQuery(userId));
            return Ok(useraccountdetails);
        }
    }
}
