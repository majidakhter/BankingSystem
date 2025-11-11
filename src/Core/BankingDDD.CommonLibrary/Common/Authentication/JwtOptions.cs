namespace BankingAppDDD.Common.Authentication
{
    public class JwtOptions
    {
        public required string SecretKey { get; set; }
        public required string Issuer { get; set; }
        public int ExpiryMinutes { get; set; }
        public bool ValidateLifetime { get; set; }
        public bool ValidateAudience { get; set; }   
        public required string ValidAudience { get; set; }
    }
}