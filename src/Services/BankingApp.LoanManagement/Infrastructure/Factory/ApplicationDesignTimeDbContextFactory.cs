using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingApp.LoanManagement.Infrastructure.Factory
{

    public  class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CreditMgmtDbContext>
    {
        public CreditMgmtDbContext CreateDbContext(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
           
            var optionsBuilder = new DbContextOptionsBuilder<CreditMgmtDbContext>();
            var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
            optionsBuilder.UseNpgsql(connectionString);
            return new CreditMgmtDbContext(optionsBuilder.Options, builder.Environment);

        }
    }
}
