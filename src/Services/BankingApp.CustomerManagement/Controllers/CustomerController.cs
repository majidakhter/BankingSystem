using BankingAppDDD.Common.Types;
using BankingAppDDD.CustomerManagement.Application.Customers.Commands;
using BankingAppDDD.CustomerManagement.Application.Customers.Models;
using BankingAppDDD.CustomerManagement.Application.Customers.Queries;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingAppDDD.CustomerManagement.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/customer")]
    public class CustomerController : ControllerBase
    {
        readonly IMediator _mediator;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        public CustomerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AddCustomer([FromBody] AddCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("updatecustomer")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateCustomer([FromBody] UpdateCustomerCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("customerdetails")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<CustomerDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _mediator.Send(new GetCustomerQuery());
            return Ok(customers);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [MapToApiVersion(ApiVersions.V2)]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(CustomerDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCustomerById(Guid id)
        {
            var customer = await _mediator.Send(new GetCustomerQueryById(id));
            return Ok(customer);
        }
    }
}
