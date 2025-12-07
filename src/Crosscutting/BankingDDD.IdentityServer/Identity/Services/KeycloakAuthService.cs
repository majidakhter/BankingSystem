using BankingAppDDD.Common.Authentication;
using BankingAppDDD.Identity.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BankingAppDDD.Identity.Services
{
    public class KeycloakAuthService : IKeycloakAuthService
    {
        private readonly IJwtHandler _jwtHandler;
        //private IMongoService _mongoService;
        //private readonly IOptionsMonitor<DomainModel.PolyConfigSettings> _policyConfigSettings;
       // private readonly ILogger _logger;
       // private readonly IConfiguration _configuration;

        public KeycloakAuthService(IJwtHandler jwtHandler)
        {
            _jwtHandler = jwtHandler;
           // _mongoService = mongoService;
           // _configuration = configuration;
          //  _logger = logger;
           // _refreshTokenRepository = refreshTokenRepository;
        }


        public async Task<JsonWebToken> SignInAsync(string userName, string password)
        {
            var jwt =await _jwtHandler.GetToken(userName, password);
            Random r = new Random();
            int tokenversion = r.Next();
            var request = new RefreshTokenRequest { UserId = userName, TokenVersion= tokenversion, Token = jwt.RefreshToken};
            //await _mongoService.SaveRefreshTokenAsync(request);
            return jwt;
        }
        
    }
}