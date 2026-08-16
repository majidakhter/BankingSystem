using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    /// <summary>
    /// Immutable log of every action, regulatory compliance
    /// </summary>
    [Route("api/v{version:apiVersion}/audit")]
    [ApiController]
    public class AuditController : ControllerBase
    {
    }
}
