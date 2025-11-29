using MongoDB.Bson.Serialization;

namespace BankingAppDDD.Common.Mongo.Interfaces.Collection
{
    public interface IMongoCollectionSerializationMap<T>
    {
        Action<BsonClassMap<T>> SerializationClassMap();
    }
}
