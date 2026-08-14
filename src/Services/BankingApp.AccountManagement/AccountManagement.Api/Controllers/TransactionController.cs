using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BankingApp.AccountManagement.Application.Transfer.Models;
using BankingApp.AccountManagement.Application.Transfer.Queries;
using BankingApp.AccountManagement.Application.Transfers.Commands;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BankingApp.AccountManagement.Controllers
{
    /// <summary>
    /// Unified Fund Transfer and Transaction Controller with In-Line Real-Time ML & LLM Fraud Evaluation
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/transaction")]
    public class TransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<TransactionController> _logger;

        public TransactionController(IMediator mediator, ILogger<TransactionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Single unified endpoint executing in-line ML & LLM Ensemble Fraud Evaluation and Fund Transfer initiation.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Transfer([FromBody] TransferFundsCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new
            {
                Status = "Transfer Initiated",
                Message = "Fund transfer has been evaluated and initiated successfully. Money movement is being processed via central clearing network.",
                Success = result,
                Timestamp = DateTime.UtcNow
            });
        }

        [HttpGet("transactionlist/{accountId}/{startDate}/{endDate}")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(List<TransferDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAllTransaction(Guid accountId, DateTime startDate, DateTime endDate)
        {
            var result = await _mediator.Send(new GetTransactionDetailsQuery(accountId, startDate, endDate));
            return Ok(result);
        }

        [HttpGet("gettransactionbyaccountid/{accountId}")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(List<TransferDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTransactionByAccountId(Guid accountId)
        {
            var result = await _mediator.Send(new GetTransactionByAccountIdQuery(accountId));
            return Ok(result);
        }

        [HttpGet("gettransactionbytransactionnumber/{transactionNo}")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(List<TransferDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetTransactionByTransactionNo(int transactionNo)
        {
            var result = await _mediator.Send(new GetTransactionByTransactionIdQuery(transactionNo));
            return Ok(result);
        }
    }
}
