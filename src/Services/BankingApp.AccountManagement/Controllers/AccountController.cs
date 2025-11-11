using BankingApp.AccountManagement.Application.Accounts.Commands;
using BankingApp.AccountManagement.Application.Accounts.Models;
using BankingApp.AccountManagement.Application.Accounts.Queries;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {

        readonly IMediator _mediator;
        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("api/Account/AddAccount")]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddAccount([FromBody] AddAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        } 

        [HttpPost("api/Account/Deposit")]
        //[Authorize(Roles = "Customer")]
        [Authorize]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deposit([FromBody] DepositCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("api/Account/Withdraw")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("api/Account/CloseAccount")]
        [Authorize(Roles = "Accountant")]
        public async Task<ActionResult> CloseAccount([FromBody] CloseAccountCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("api/Account/AddBeneficiary")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddBeneficiary([FromBody] AddBeneficiaryCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("api/Account/GetAccountStatus")]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(AccountStatusDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAccountStatus(AccountStatusQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
