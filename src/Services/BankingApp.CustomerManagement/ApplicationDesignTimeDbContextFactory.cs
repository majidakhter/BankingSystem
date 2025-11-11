using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankingAppDDD.CustomerManagement
{
    /// <summary>
    /// 
    /// </summary>
    public  class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<CustomerDbContext>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CustomerDbContext CreateDbContext(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
           
            var optionsBuilder = new DbContextOptionsBuilder<CustomerDbContext>();
            var connectionString = builder.Configuration["DbContextSettings:ConnectionString"];
            optionsBuilder.UseNpgsql(connectionString);
            return new CustomerDbContext(optionsBuilder.Options, builder.Environment);

        }
    }
}
