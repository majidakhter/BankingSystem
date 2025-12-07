using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Configuration;
using System.Net.Sockets;

namespace BankingAppDDD.Common.Extension
{
    public static class MongoExtensions
    {

        public static void AddDependencyServices(this IServiceCollection services, IConfigurationManager _configuration)
        {
            if (_configuration is null)
                throw new ArgumentNullException(nameof(_configuration));
            services.AddSingleton<IMongoClient>(s =>
                            new MongoClient(_configuration["MongoDbSettings:MongoConnectionString"])
                );
            void SocketConfigurator(Socket s) => s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
            MongoIdentity identity = new MongoInternalIdentity(_configuration["MongoDbSettings:Database"], _configuration["MongoDbSettings:UserName"]);
            MongoIdentityEvidence evidence = new PasswordEvidence(_configuration["MongoDbSettings:Password"]);
            services.AddSingleton<IMongoClient>(s =>
                {
                    var settings = new MongoClientSettings
                    {
                        Credential = new MongoCredential(_configuration["MongoDbSettings:Mechanism"], identity, evidence),
                        SocketTimeout = TimeSpan.FromSeconds(60),
                        MaxConnectionIdleTime = TimeSpan.FromSeconds(60),
                        ClusterConfigurator = builder =>
                        builder.ConfigureTcp(tcp => tcp.With(socketConfigurator: (Action<Socket>)SocketConfigurator))
                    };

                    return new MongoClient(settings);
                });
        }

    }

}

