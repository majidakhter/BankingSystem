using System.Security.Claims;

namespace BankingAppDDD.Common.Messages

{
    public class Users
    {
        public string? UserName { get; set; }
        public string? Role { get; set; }

        public IEnumerable<Users> GetUsers(IEnumerable<Claim>? claimdata, IEnumerable<string> claimroles)
        {

            var TokenInfo = new List<Users>();
            var allRoles = new List<string> { "Admin", "Manager", "Customer", "Accountant", "Operator", "Underwriter" };
            IEnumerable<string> roleval = claimroles.Intersect(allRoles);
            foreach (var item in roleval)
            {
                TokenInfo.Add(new Users { UserName = claimdata?.First(c => c.Type == "preferred_username").Value, Role = item });
            }
            return TokenInfo;
        }

    }
}
