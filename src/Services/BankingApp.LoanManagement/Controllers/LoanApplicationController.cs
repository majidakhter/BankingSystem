using BankingApp.LoanManagement.Application.DebtorInfosCommand;
using BankingApp.LoanManagement.Application.LoanApplicationCommands;
using BankingApp.LoanManagement.Application.LoanApplicationModels;
using BankingApp.LoanManagement.Application.LoanApplicationQueries;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.LoanManagement.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/loanapplication")]
    public class LoanApplicationController : ControllerBase
    {
        readonly IMediator _mediator;
        public LoanApplicationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost, Route("createloanapplication")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Operator")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> CreateLoanApplication([FromBody] LoanApplicationSubmittedCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut, Route("evaluateloanapplication")]
        [MapToApiVersion(ApiVersions.V2)]
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

        [HttpPut, Route("acceptloanapplication")]
        [MapToApiVersion(ApiVersions.V2)]
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

        [HttpPut, Route("rejectloanapplication")]
        [MapToApiVersion(ApiVersions.V2)]
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

        [HttpGet("getloanbyid/{loanapplicationid}")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(List<LoanApplicationDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(Guid loanapplicationid)
        {
            var branches = await _mediator.Send(new GetLoanApplicationsQueryById(loanapplicationid));
            return Ok(branches);
        }

        [HttpGet("getloanbyparam/{applicationnumber}/{customeridentifier}/{decisionbyid}/{registeredbyid}")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(LoanApplicationSearchCriteriaDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetLoanApplicationByOtherParam(string aapplicationnumber, Guid customeridentifier, Guid decisionbyid, Guid registeredbyid)
        {
            var branches = await _mediator.Send(new GetLoanApplicationByOtherParam(aapplicationnumber, customeridentifier, decisionbyid, registeredbyid));
            return Ok(branches);
        }

        [HttpPost, Route("createdebtorInfo")] 
        [MapToApiVersion(ApiVersions.V2)]
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
