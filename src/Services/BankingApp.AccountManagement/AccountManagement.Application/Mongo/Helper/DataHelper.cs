using BankingAppDDD.Common.Helpers;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace BankingAppDDD.AccountManagement.Application.Mongo.Helper
{
    public static class DataHelper
    {
        public static Guid GetAccountKey(Guid accountId, int accountversion, string dataVersion)
        {

            JObject liteItemKey = JObject.FromObject(new
            {
                Id = accountId.ToString().ToLower(CultureInfo.CurrentCulture),
                UserVersion = accountversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return AccountNumberGenerator.GenerateUniqueGuid(liteItemKey.ToString(0));
        }
        public static string GetAccountReadableKey(Guid accountId, int accountversion, string? dataVersion)
        {

            JObject itemKey = JObject.FromObject(new
            {
                Type = "Account",
                Id = accountId.ToString().ToLower(CultureInfo.CurrentCulture),
                TokenVersion = accountversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return itemKey.ToString();
        }

        public static Guid GetBranchKey(Guid branchId, int branchversion, string dataVersion)
        {

            JObject liteItemKey = JObject.FromObject(new
            {
                Id = branchId.ToString().ToLower(CultureInfo.CurrentCulture),
                UserVersion = branchversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return AccountNumberGenerator.GenerateUniqueGuid(liteItemKey.ToString(0));
        }
        public static string GetBranchReadableKey(Guid branchId, int branchversion, string? dataVersion)
        {

            JObject itemKey = JObject.FromObject(new
            {
                Type = "Branch",
                Id = branchId.ToString().ToLower(CultureInfo.CurrentCulture),
                TokenVersion = branchversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return itemKey.ToString();
        }
        public static Guid GetBankKey(Guid bankId, int bankversion, string dataVersion)
        {

            JObject liteItemKey = JObject.FromObject(new
            {
                Id = bankId.ToString().ToLower(CultureInfo.CurrentCulture),
                UserVersion = bankversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return AccountNumberGenerator.GenerateUniqueGuid(liteItemKey.ToString(0));
        }
        public static string GetBankReadableKey(Guid bankId, int bankversion, string? dataVersion)
        {

            JObject itemKey = JObject.FromObject(new
            {
                Type = "Bank",
                Id = bankId.ToString().ToLower(CultureInfo.CurrentCulture),
                TokenVersion = bankversion.ToString().ToLower(CultureInfo.CurrentCulture),
                VersionId = dataVersion
            });

            return itemKey.ToString();
        }

    }
}
