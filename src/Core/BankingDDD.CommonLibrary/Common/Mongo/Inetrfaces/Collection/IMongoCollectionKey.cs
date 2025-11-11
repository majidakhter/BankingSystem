

namespace BankingAppDDD.Common.Mongo.Interfaces.Collection
{
    public interface IMongoCollectionKey<T>
    {
        T Id { get; set; }
    }
}
