using BankingAppDDD.Identity.Model;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace BankingAppDDD.Identity.Helper
{
    public static class DataHelper
    {
        public static Guid GetRefreshTokenKey(RefreshTokenRequest request, string? dataVersion)
        {
            JObject liteItemKey = JObject.FromObject(new
            {
                Id = request.UserId?.ToLower(CultureInfo.CurrentCulture),
                TokenVersion = request.TokenVersion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return GenerateUniqueGuid(liteItemKey.ToString(0));
        }
        public static string GetSolutionRefreshTokenReadableKey(RefreshTokenRequest request, string? dataVersion)
        {
            JObject itemKey = JObject.FromObject(new
            {
                Type = "RefreshToken",
                Id = request.UserId?.ToLower(CultureInfo.CurrentCulture),
                TokenVersion = request?.TokenVersion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return itemKey.ToString();
        }
        private static Guid GenerateUniqueGuid(string key)
        {
            Guid retVal = Guid.Empty;

            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = encoding.GetBytes(key);

            MD5 md5 = MD5.Create();
            retVal = new Guid(md5.ComputeHash(stream));

            return retVal;
        }
    }
}
