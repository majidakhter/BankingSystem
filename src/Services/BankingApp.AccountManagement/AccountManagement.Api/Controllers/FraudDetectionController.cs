using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    /// <summary>
    /// Real-time transaction scoring, pattern analysis
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class FraudDetectionController : ControllerBase
    {
    }
}
