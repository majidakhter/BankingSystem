using MongoDB.Bson.Serialization;
using System;

namespace BankingAppDDD.Common.Mongo.Interfaces.Collection
{
    public interface IMongoCollectionSerializationMap<T>
    {
        Action<BsonClassMap<T>> SerializationClassMap();
    }
}
