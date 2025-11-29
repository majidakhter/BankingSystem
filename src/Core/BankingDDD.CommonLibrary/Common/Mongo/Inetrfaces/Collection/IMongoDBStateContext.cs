
namespace BankingAppDDD.Common.Mongo.Interfaces.Collection
{
    public interface IMongoDBStateContext
    {
        string? CollectionName { get; set; }
        TimeSpan TTLExpiration { get; set; }
        string? ExpirationFieldName { get; set; }
    }
}
