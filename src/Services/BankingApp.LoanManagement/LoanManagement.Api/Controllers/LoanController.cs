using BankingApp.LoanManagement.Application.LoanApplicationQueries;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Domains.LoanApplications.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.LoanManagement.Controllers
{
    [Route("api/v{version:apiVersion}/loan")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        readonly IMediator _mediator;
        public LoanController(IMediator mediator)
        {
            _mediator = mediator;
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
    }
}
