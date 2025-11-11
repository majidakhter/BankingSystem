using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.AccountManagement
{

    public  class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
    {
        public AccountDbContext CreateDbContext(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");

            var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
            var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
            optionsBuilder.UseNpgsql(connectionString);
            return new AccountDbContext(optionsBuilder.Options, builder.Environment);

        }
    }
}
