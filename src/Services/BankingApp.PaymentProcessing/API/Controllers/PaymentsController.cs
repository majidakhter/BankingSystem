using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BankingApp.PaymentProcessing.Application.Command;
using BankingApp.PaymentProcessing.Model;
using BankingAppDDD.Common.Types;
using BankingAppDDD.Infrastructures.ActionResults;
using BankingAppDDD.PaymentProcessing.Model;
using PaymentRequest = BankingAppDDD.PaymentProcessing.API.Controllers.Requests.PaymentRequest;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BankingAppDDD.PaymentManagement.Controllers
{
    /// <summary>
    /// Unified Payment Controller for Razorpay, PhonePe, and Payout Processing
    /// </summary>
    [Route("api/v{version:apiVersion}/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string _webhookSecret = "YOUR_WEBHOOK_SECRET";
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Initiates PhonePe Payment Standard Checkout flow
        /// </summary>
        [HttpPost("request-phonepe")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(PaymentInitiationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestPhonePePayment([FromBody] PaymentRequest request)
        {
            var result = await _mediator.Send(new RequestPhonePePaymentCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Initiates Razorpay Payment Order Creation flow
        /// </summary>
        [HttpPost("request-razorpay")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(PaymentInitiationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RequestRazorpay([FromBody] PaymentRequest request)
        {
            var result = await _mediator.Send(new RequestRazorPaymentCommand(request));
            return Ok(result);
        }

        /// <summary>
        /// Completes and captures Razorpay payment
        /// </summary>
        [HttpPost("complete-razorpay")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CompleteRazorpay([FromBody] CompleteRazorPayCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Unified endpoint for Bill Payment & Recharges using chosen gateway (PhonePe / Razorpay)
        /// </summary>
        [HttpPost("pay-bill")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(PaymentInitiationResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PayBill([FromBody] PaymentRequest request)
        {
            if (request == null)
            {
                return BadRequest("Invalid payment request payload.");
            }

            if (request.GatewayProvider?.ToUpper() == "PHONEPE")
            {
                var phonePeResult = await _mediator.Send(new RequestPhonePePaymentCommand(request));
                return Ok(phonePeResult);
            }
            else
            {
                var razorResult = await _mediator.Send(new RequestRazorPaymentCommand(request));
                return Ok(razorResult);
            }
        }

        // 1. Initiate Payout Endpoint (Billpayment via RazorpayX example)
        [HttpPost("payout")]
        [MapToApiVersion(ApiVersions.V2)]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendPayout([FromBody] RequestRazorPaymentCommand command)
        {
            PaymentInitiationResult result = await _mediator.Send(command);
            return Ok(result);
        }

        // 2. Webhook Endpoint Listener to Consume Transfer Updates
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var jsonPayload = await reader.ReadToEndAsync();

            string signature = Request.Headers["X-Razorpay-Signature"];

            if (!VerifySignature(jsonPayload, signature, _webhookSecret))
            {
                return BadRequest(new { status = "Invalid signature" });
            }

            var data = JObject.Parse(jsonPayload);
            string eventName = data["event"]?.ToString();

            if (eventName == "payout.processed")
            {
                var payoutEntity = data["payload"]?["payout"]?["entity"];
                string utr = payoutEntity?["utr"]?.ToString();
                string payoutId = payoutEntity?["id"]?.ToString();
            }

            return Ok(new { status = "handled" });
        }

        private bool VerifySignature(string payload, string signature, string secret)
        {
            var encoding = new UTF8Encoding();
            byte[] keyBytes = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(payload);
            using var hmacsha256 = new HMACSHA256(keyBytes);
            byte[] hashMessage = hmacsha256.ComputeHash(messageBytes);
            string generatedSignature = BitConverter.ToString(hashMessage).Replace("-", "").ToLower();
            return generatedSignature == signature;
        }
    }
}
