using BankingApp.AccountManagement.Application.Customers.Models;
using BankingApp.AccountManagement.Application.Customers.Queries;
using BankingAppDDD.Common.Types;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/customer")]

    public class CustomerController : ControllerBase
    {
        readonly IMediator _mediator;
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("getcustomeraccountcount/{customerid}")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(CustomerAccountDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerAccountCountById(Guid customerid)
        {
            var customer = await _mediator.Send(new GetCustomerAccountCountById(customerid));
            return Ok(customer);
        }
    }
}
