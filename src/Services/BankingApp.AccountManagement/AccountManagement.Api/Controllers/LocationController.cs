using BankingAppDDD.Common.Types;
using BankingAppDDD.MongoService.Application.Mongo;
using BankingAppDDD.MongoService.Mongo.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BankingApp.AccountManagement.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/location")]
    public class LocationController : ControllerBase
    {
        private readonly IAccountMongoService _mongoService;

        public LocationController(IAccountMongoService mongoService)
        {
            _mongoService = mongoService;
        }

        [HttpGet("countries")]
        [MapToApiVersion(ApiVersions.V2)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<CountryReadModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await _mongoService.GetCountriesAsync();
            return Ok(countries);
        }

        [HttpGet("states")]
        [MapToApiVersion(ApiVersions.V2)]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<StateReadModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStates()
        {
            var states = await _mongoService.GetStatesAsync();
            return Ok(states);
        }
    }
}
