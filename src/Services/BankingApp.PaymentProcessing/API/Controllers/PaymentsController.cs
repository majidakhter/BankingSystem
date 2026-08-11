using BankingAppDDD.Common.Types;
using BankingAppDDD.Infrastructures.ActionResults;
using BankingAppDDD.PaymentProcessing.Application.RequestingPayment;
using MediatR;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BankingAppDDD.PaymentManagement.Controllers
{
    /// <summary>
    /// Immutable log of every action, regulatory compliance
    /// </summary>
    [Route("api/v{version:apiVersion}/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly string _webhookSecret = "YOUR_WEBHOOK_SECRET";
        readonly IMediator _mediator;
        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }
        // 1. Initiate Payout Endpoint (UPI / NEFT / RTGS via RazorpayX example)
        [HttpPost("payout")]
        [MapToApiVersion(ApiVersions.V2)]
        //[Authorize(Roles = "Accountant")]
        [ProducesResponseType(typeof(CreatedResultEnvelope), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SendPayout([FromBody] RequestPaymentCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        // 2. Webhook Endpoint Listener to Consume Transfer Updates
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            using var reader = new StreamReader(Request.Body);
            var jsonPayload = await reader.ReadToEndAsync();

            // Retrieve signature header (X-Razorpay-Signature or PhonePe equivalent)
            string signature = Request.Headers["X-Razorpay-Signature"];

            if (!VerifySignature(jsonPayload, signature, _webhookSecret))
            {
                return BadRequest(new { status = "Invalid signature" });
            }

            var data = JObject.Parse(jsonPayload);
            string eventName = data["event"]?.ToString();

            // Handle specific payout status events
            if (eventName == "payout.processed")
            {
                var payoutEntity = data["payload"]?["payout"]?["entity"];
                string utr = payoutEntity?["utr"]?.ToString();
                string payoutId = payoutEntity?["id"]?.ToString();

                // TODO: Update your internal database status to Success/Completed using payoutId & UTR
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
