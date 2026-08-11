using BankingAppDDD.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace BankingAppDDD.UserManagement.Application.Mongo.Helper
{
    public static class DataHelper
    {
        public static Guid GetUserKey(Guid userId,int userversion, string dataVersion)
        {
            
            JObject liteItemKey = JObject.FromObject(new
            {
                Id = userId.ToString().ToLower(CultureInfo.CurrentCulture),
                UserVersion = userversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return AccountNumberGenerator.GenerateUniqueGuid(liteItemKey.ToString(0));
        }
        public static string GetUserReadableKey(Guid userId, int userversion, string? dataVersion)
        {
           
            JObject itemKey = JObject.FromObject(new
            {
                Type = "User",
                Id = userId.ToString().ToLower(CultureInfo.CurrentCulture),
                TokenVersion = userversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return itemKey.ToString();
        }
        
        
    }
}
