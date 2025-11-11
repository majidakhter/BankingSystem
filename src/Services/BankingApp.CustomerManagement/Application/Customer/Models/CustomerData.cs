using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAppDDD.CustomerManagement.Application.Customers.Models
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="Email"></param>
    /// <param name="Name"></param>
    /// <param name="phoneno"></param>
    /// <param name="dateOfBirth"></param>
    /// <param name="customerType"></param>
    public record class CustomerData(string Email,
    string Name,
    string phoneno,
    DateOnly dateOfBirth,
    int customerType);
    
}
