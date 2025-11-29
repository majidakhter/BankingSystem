using BankingApp.AccountManagement.Application.Branches.Commands;
using BankingApp.AccountManagement.Application.Branches.Model;
using BankingApp.AccountManagement.Application.Branches.Queries;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        readonly IMediator _mediator;
        public BranchController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddBranch([FromBody] AddBranchCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet, Route("branchDetails")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(List<BranchDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var branches = await _mediator.Send(new GetBranchQuery());
            return Ok(branches);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(BranchDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBranchById(Guid id)
        {
            var branches = await _mediator.Send(new GetBranchQueryById(id));
            return Ok(branches);
        }
    }
}
