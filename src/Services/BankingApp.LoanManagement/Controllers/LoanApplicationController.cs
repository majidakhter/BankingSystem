using BankingApp.LoanManagement.Application.DebtorInfosCommand;
using BankingApp.LoanManagement.Application.LoanApplicationCommands;
using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Application.LoanApplicationQueries;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.LoanManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanApplicationController : ControllerBase
    {
        readonly IMediator _mediator;
        public LoanApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("CreateLoanApplication")]
        [Authorize(Roles = "Operator")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateLoanApplication([FromBody] LoanApplicationSubmittedCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut, Route("EvaluateLoanApplication")]
        [Authorize(Roles = "Underwriter")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
        public async Task<ActionResult> EvaluateLoanApplication([FromBody] EvaluateLoanApplicationCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new ApiResponse { Success = true, Message = "Resource updated successfully." });
            }
            else
            {
                // Handle cases where the update fails (e.g., resource not found, validation errors)
                return BadRequest(new ApiResponse { Success = false, Message = "Failed to update resource." });
            }
        }

        [HttpPut, Route("AcceptLoanApplication")]
        [Authorize(Roles = "Underwriter")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
        public async Task<ActionResult> AcceptLoanApplication([FromBody] AcceptLoanApplicationCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new ApiResponse { Success = true, Message = "Resource updated successfully." });
            }
            else
            {
                // Handle cases where the update fails (e.g., resource not found, validation errors)
                return BadRequest(new ApiResponse { Success = false, Message = "Failed to update resource." });
            }
        }

        [HttpPut, Route("RejectLoanApplication")]
        [Authorize(Roles = "Underwriter")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status200OK)]
        public async Task<ActionResult> RejectLoanApplication([FromBody] RejectLoanApplicationCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
            {
                return Ok(new ApiResponse { Success = true, Message = "Resource updated successfully." });
            }
            else
            {
                // Handle cases where the update fails (e.g., resource not found, validation errors)
                return BadRequest(new ApiResponse { Success = false, Message = "Failed to update resource." });
            }
        }

        [HttpGet("GetLoanById/{loanApplicationid}")]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(List<LoanApplicationDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid loanApplicationid)
        {
            var branches = await _mediator.Send(new GetLoanApplicationsQueryById(loanApplicationid));
            return Ok(branches);
        } 

        [HttpGet("GetLoanByParam/{applicationNumber}/{customerIdentifier}/{decisionById}/{registeredById}")]
        [ProducesResponseType(typeof(LoanApplicationSearchCriteriaDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLoanApplicationByOtherParam(string applicationNumber,Guid customerIdentifier, Guid decisionById, Guid registeredById)
        {
            var branches = await _mediator.Send(new GetLoanApplicationByOtherParam(applicationNumber, customerIdentifier, decisionById, registeredById));
            return Ok(branches);
        }

        [HttpPost, Route("CreateDebtorInfo")]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateDebtorInfos([FromBody] CreateDebtorInfoCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
