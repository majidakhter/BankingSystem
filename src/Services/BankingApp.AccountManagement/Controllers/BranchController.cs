using BankingApp.AccountManagement.Application.Branches.Commands;
using BankingApp.AccountManagement.Application.Branches.Model;
using BankingApp.AccountManagement.Application.Branches.Queries;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/branch")]
    public class BranchController : ControllerBase
    {
        readonly IMediator _mediator;
        public BranchController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddBranch([FromBody] AddBranchCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet, Route("branchdetails")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(List<BranchDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var branches = await _mediator.Send(new GetBranchQuery());
            return Ok(branches);
        }

        [HttpGet("{id}")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(BranchDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBranchById(Guid id)
        {
            var branches = await _mediator.Send(new GetBranchQueryById(id));
            return Ok(branches);
        }
    }
}
