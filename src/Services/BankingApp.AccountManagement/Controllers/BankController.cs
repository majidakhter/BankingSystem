using BankingApp.AccountManagement.Application.Banks.Commands;
using BankingApp.AccountManagement.Application.Banks.Models;
using BankingApp.AccountManagement.Application.Banks.Queries;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankController : ControllerBase
    {
        readonly IMediator _mediator;
        public BankController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddBank([FromBody] AddBankCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(BankDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid id)
        {
            var banks = await _mediator.Send(new GetBanksQuery(id));
            return Ok(banks);
        }
    }
}
