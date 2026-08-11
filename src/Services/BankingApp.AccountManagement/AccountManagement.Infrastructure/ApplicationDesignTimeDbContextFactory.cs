using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace BankingApp.AccountManagement
{
    public class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AccountDbContext>
    {
        public AccountDbContext CreateDbContext(string[] args)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            // 2. Get the connection string from the configuration
            var connectionString = configuration["DbContextSettings:ConnectionString"];

            // 3. Configure DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<AccountDbContext>();
            optionsBuilder.UseNpgsql(connectionString);

            // 4. Return the context (Pass configuration instead of builder.Environment if needed)
            return new AccountDbContext(optionsBuilder.Options, null);

        }
    }
}
