namespace BankingAppDDD.CustomerManagement.Application.Customers.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="CustomerId"></param>
    /// <param name="Email"></param>
    /// <param name="Name"></param>
    /// <param name="phoneno"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="customerType"></param>
    public record class CustomerUpdateData(Guid CustomerId, string Email,
    string Name,
    string phoneno,
    DateOnly dateOfBirth,
    int customerType);

}
