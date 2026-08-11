using Microsoft.AspNetCore.Http;

namespace BankingAppDDD.Domains.Users.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="UserId"></param>
    /// <param name="Email"></param>
    /// <param name="FirstName"></param>
    /// <param name="LastName"></param>
    /// <param name="PhoneNo"></param>
    /// <param name="DateOfBirth"></param>
    /// <param name="UserType"></param>
    /// <param name="Gender"></param>
    /// <param name="Ssn"></param>
    /// <param name="ProfileImage"></param>
    public record class UserUpdateData(Guid UserId, string Email,
    string FirstName,
    string LastName,
    string PhoneNo,
    DateOnly DateOfBirth,
    int UserType,
    string Gender,
    string Ssn,
    IFormFile ProfileImage);

}
