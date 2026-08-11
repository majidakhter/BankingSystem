using BankingApp.AccountManagement.Application.Accounts.Commands;
using BankingApp.AccountManagement.Application.Accounts.Queries;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Domains.Accounts.Models;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    /// <summary>
    /// Balance enquiry, account status, holds
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/account")]
    public class AccountController : ControllerBase
    {

        readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        } 


        [HttpPost("addaccount")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddAccount([FromBody] AddAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        
       

        [HttpPost("deposit")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deposit([FromBody] DepositCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("withdraw")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("closeaccount")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CloseAccount([FromBody] CloseAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("addbeneficiary")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddBeneficiary([FromBody] AddBeneficiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("getaccountstatus/{accountid}")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(AccountStatusDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountStatus(Guid accountid)
        {
            var result = await _mediator.Send(new AccountStatusQuery(accountid));
            return Ok(result);
        }

        [HttpGet("accountlist")]
        [MapToApiVersion(ApiVersions.V2)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<AccountDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAllAccounts()
        {
            var result = await _mediator.Send(new GetAccountDetailsQuery());
            return Ok(result);
        }


        [HttpGet("getaccount/{customerid}")]
        [MapToApiVersion(ApiVersions.V2)]
       // [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(List<AccountDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountByCustomerId(Guid customerid)
        {
            var result = await _mediator.Send(new GetAccountsByCustomerIdQuery(customerid));
            return Ok(result);
        }

        [HttpGet("getcurrentbalance/{accountid}")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountCurrentBalance(Guid accountid)
        {
            var result = await _mediator.Send(new GetAccountCurrentBalanceQuery(accountid));
            return Ok(result);
        }

        /* [HttpGet("{accountId:guid}")]
    public async Task<IActionResult> GetAccount(Guid accountId)
    {
        var account = await _accounts.GetAccountByIdAsync(accountId);
        if (account is null) return NotFound();

        // Clients can only see their own accounts
        if (_currentUser.IsInRole("Cliente") && account.CustomerId != _currentUser.CustomerId)
            return Forbid();

        return Ok(account);
    }*/
    }
}
