using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BankingApp.LoanManagement.Infrastructure.Factory
{

    public  class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CreditMgmtDbContext>
    {
        public CreditMgmtDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            // 2. Get the connection string from the configuration
            var connectionString = configuration["DbContextSettings:ConnectionString"];

            // 3. Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<CreditMgmtDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            // 4. Return the context (Pass configuration instead of builder.Environment if needed)
            return new CreditMgmtDbContext(optionsBuilder.Options, null);

        }
    }
}
