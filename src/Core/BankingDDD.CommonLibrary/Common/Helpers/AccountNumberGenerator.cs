using System.Security.Cryptography;
using System.Text;

namespace BankingAppDDD.Common.Helpers
{
    /// <summary>
    /// Utility class for generating dynamic account numbers and transaction numbers across bounded contexts
    /// </summary>
    public static class AccountNumberGenerator
    {
        /// <summary>
        /// Generates a dynamic 7-digit Account Number deterministically based on a user's Guid
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <returns>A 7-digit integer account number between 1000000 and 1899999</returns>
        public static int GenerateDynamicAccountNumber(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return 1000004;
            }
            byte[] bytes = userId.ToByteArray();
            int hash = Math.Abs(BitConverter.ToInt32(bytes, 0) ^ BitConverter.ToInt32(bytes, 4));
            return 1000000 + (hash % 900000);
        }

        /// <summary>
        /// Generates a dynamic 8-digit Transaction Number deterministically based on a user's Guid
        /// </summary>
        /// <param name="userId">The unique identifier of the user</param>
        /// <returns>A 7-digit integer account number between 10000000 and 18999999</returns>
        public static int GenerateDynamicTransactionNumber(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return 1000004;
            }
            byte[] bytes = userId.ToByteArray();
            int hash = Math.Abs(BitConverter.ToInt32(bytes, 0) ^ BitConverter.ToInt32(bytes, 4));
            return 10000000 + (hash % 9900000);
        }

        /// <summary>
        /// To generate unique guid which will be used for mongo data helper
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GenerateUniqueGuid(string key)
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
