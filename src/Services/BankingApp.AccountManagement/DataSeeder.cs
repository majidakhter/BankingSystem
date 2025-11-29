using BankingApp.AccountManagement.Core.Accounts.Entities;
using BankingApp.AccountManagement.Core.Banks.Entities;
using BankingApp.AccountManagement.Core.Branches.Entities;
using BankingApp.AccountManagement.Core.Customers.Entities;

namespace BankingApp.AccountManagement
{
    public static class WebApplicationExtension
    {
        public static WebApplication Seed(this WebApplication webApp, Dictionary<string, List<object>> allData)
        {
            using (var serviceScope = webApp.Services.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<AccountDbContext>();
                bool record = false;
                if (allData.TryGetValue("Bank", out var bankData))
                {
                    if (context.Banks.Any())
                    {
                        record = true;
                    }
                    else
                    {
                        context.AddRange(bankData.Cast<Bank>().Select(item => item));
                        record = false;
                    }

                }

                if (allData.TryGetValue("Branches", out var branchData))
                {
                    if (context.Branches.Any())
                    {
                        record = true;
                    }
                    else
                    {
                        context.AddRange(branchData.Cast<Branch>().Select(item => item));
                        record = false;

                    }

                }
                if (allData.TryGetValue("Accounts", out var accountData))
                {
                    if (context.Accounts.Any())
                    {
                        record = true;
                    }
                    else
                    {
                        context.AddRange(accountData.Cast<Account>().Select(item => item));
                        record = false;
                    }

                }
                if (allData.TryGetValue("BankCustomers", out var bankCustomerData))
                {
                    if (context.Customers.Any())
                    {
                        record = true;
                    }
                    else
                    {
                        context.AddRange(bankCustomerData.Cast<Customer>().Select(item => item));
                        record = false;

                    }

                }

                if (!record)
                    context.SaveChanges();
            }
            return webApp;
        }
    }
}
