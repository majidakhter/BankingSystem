using BankingApp.LoanManagement.Application.OperatorsCommand;
using BankingApp.LoanManagement.Application.OperatorsModel;
using BankingApp.LoanManagement.Application.OperatorsQueries;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.LoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OperatorController : ControllerBase
    {
        readonly IMediator _mediator;
        public OperatorController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddOperator([FromBody] AddOperatorCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<OperatorDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult> GetOperators(GetOperatorQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
