using BankingApp.AccountManagement.Application.Transfer.Models;
using BankingApp.AccountManagement.Application.Transfer.Queries;
using BankingApp.AccountManagement.Application.Transfers.Commands;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Infrastructures.ActionResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

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
        private readonly IDistributedCache? _cache;

        public TransactionController(
            IMediator mediator,
            ILogger<TransactionController> logger,
            IDistributedCache? cache = null)
        {
            _mediator = mediator;
            _logger = logger;
            _cache = cache;
        }

        /// <summary>
        /// Single unified endpoint executing in-line ML & LLM Ensemble Fraud Evaluation and Fund Transfer initiation.
        /// <summary>
        /// Single unified endpoint executing in-line ML & LLM Ensemble Fraud Evaluation and Fund Transfer initiation.
        /// Hardened with Redis-based Idempotency & Deduplication to prevent message duplication and double debits.
        /// </summary>
        [HttpPost]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Transfer(
            [FromBody] TransferFundsCommand command,
            [FromHeader(Name = "X-Idempotency-Key")] string? idempotencyKey = null)
        {
            // 1. Generate or resolve Idempotency Key
            var rawKey = !string.IsNullOrWhiteSpace(idempotencyKey)
                ? idempotencyKey.Trim()
                : $"transfer:{command.senderAccountId}:{command.receiverAccountId}:{command.amount}:{command.transferType}:{command.description}";

            var cacheKey = $"idempotency:{rawKey}";

            // 2. Redis Deduplication Check
            var cachedResponse = await GetCachedResponseAsync<object>(cacheKey);
            if (cachedResponse != null)
            {
                _logger.LogWarning("Redis Idempotency Check: Duplicate transfer request detected for Key {CacheKey}. Returning cached response without re-executing command.", cacheKey);
                return Ok(cachedResponse);
            }

            // 3. Execute Handler
            var result = await _mediator.Send(command);

            var responseObj = new
            {
                Status = "Transfer Initiated",
                Message = "Fund transfer has been evaluated and initiated successfully. Money movement is being processed via central clearing network.",
                Success = result,
                IdempotencyKey = rawKey,
                Timestamp = DateTime.UtcNow
            };

            // 4. Cache Response in Redis with 24-Hour TTL
            await SetCachedResponseAsync(cacheKey, responseObj, TimeSpan.FromHours(24));

            return Ok(responseObj);
        }

        #region Redis Caching Helpers

        private async Task<T?> GetCachedResponseAsync<T>(string cacheKey)
        {
            if (_cache == null) return default;
            try
            {
                var cachedData = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cachedData))
                {
                    return System.Text.Json.JsonSerializer.Deserialize<T>(cachedData);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis Cache Get error for key {CacheKey}. Bypassing cache.", cacheKey);
            }
            return default;
        }

        private async Task SetCachedResponseAsync<T>(string cacheKey, T data, TimeSpan ttl)
        {
            if (_cache == null) return;
            try
            {
                var serialized = System.Text.Json.JsonSerializer.Serialize(data);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                };
                await _cache.SetStringAsync(cacheKey, serialized, options);
                _logger.LogInformation("Redis Cache Set: Saved entry for key {CacheKey} with {TTL}s TTL.", cacheKey, ttl.TotalSeconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Redis Cache Set error for key {CacheKey}.", cacheKey);
            }
        }

        #endregion

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
