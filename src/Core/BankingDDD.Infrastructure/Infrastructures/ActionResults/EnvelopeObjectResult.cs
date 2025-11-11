using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingAppDDD.Infrastructures.ActionResults
{
    public class EnvelopeObjectResult : ObjectResult
    {
        public EnvelopeObjectResult(Envelope envelope)
            : base(envelope)
        {
            StatusCode = envelope.Status;
        }
    }
}
