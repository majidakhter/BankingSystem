using BankingAppDDD.Common.Mongo.Helper;
using BankingAppDDD.Common.Mongo.Interfaces.Collection;
using BankingAppDDD.Common.Mongo.Interfaces.Operations;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BankingAppDDD.Common.Mongo
{
    public class MongoDbContext : IMongoDbContext
    {
       
        private readonly IMongoDatabase _mongoDatabase;
        private readonly Collections configCollectionName;
        public MongoDbContext(IMongoDatabase mongodatabase, IOptions<Collections> mySettingsOptions)
        {
            _mongoDatabase = mongodatabase;
            configCollectionName = mySettingsOptions.Value;
            CreateMongoCollection();
        }

        private void CreateMongoCollection()
        {
            var configcollectionData = GetAllCollection(configCollectionName);
            List<string> dbcollectionNames = _mongoDatabase.ListCollectionNames().ToList();
            var resultcollectionname = configcollectionData.Except(dbcollectionNames);
            foreach (var item in resultcollectionname)
            {
                _mongoDatabase?.CreateCollectionAsync(item, new CreateCollectionOptions
                {
                    Capped = true,
                    MaxSize = 10485760 // 10MB
                });
            }
          //  resultcollectionname.Select(x> _mongoDatabase?.CreateCollectionAsync(x , new CreateCollectionOptions { Capped = true, MaxSize = 10485760 })); check this code
        }
        private List<string> GetAllCollection(Collections collectionval)
        {
            var collectionData = new List<string>();
            Type type = collectionval!.GetType();
            var properties = type.GetProperties();
            // collectionData = properties.Select(x => x.GetValue(collectionval).ToString()).ToList();  //check this code
            foreach (PropertyInfo property in properties)
            {
                collectionData.Add(property.GetValue(collectionval).ToString());
            }
            return collectionData;
        }

        #region Support Methods for Managing Collections

        public IMongoCollection<T> GetCollection<T>(CancellationToken cancellationToken, IMongoDBStateContext? stateContext= null) where T : class
        {
            return GetCollectionObject<T>(cancellationToken, stateContext);
        }

        //public IMongoCollection<T> GetCollectionWithClassMap<T>() where T : class, IMongoCollectionSerializationMap<T>, new()
        //{
        //    if (!BsonClassMap.IsClassMapRegistered(typeof(T)))
        //    {
        //        Action<BsonClassMap<T>> classMap = (new T()).SerializationClassMap();
        //        if (classMap != null) BsonClassMap.RegisterClassMap<T>(classMap);
        //        else BsonClassMap.RegisterClassMap<T>();
        //    }

        //    return GetCollectionObject<T>();
        //}

        //private void CreateCollection(List<string> mongoCollections)
        // {
         //foreach (var collection in mongoCollections)
        // {
         //_mongoDatabase.GetCollection<BsonDocument>(collection);
        // }
        // }

        private IMongoCollection<T> GetCollectionObject<T>(CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null)
        {
            IMongoCollection<T>? collectionObject = null;

            if (stateContext != null && !string.IsNullOrEmpty(stateContext.CollectionName))
            {
                collectionObject = _mongoDatabase.GetCollection<T>(stateContext.CollectionName);
            }
            else
            {
                collectionObject = _mongoDatabase.GetCollection<T>(typeof(T).Name);
            }

            CreateCollectionTTLIndex(collectionObject, cancellationToken, stateContext);

            return collectionObject;
        }


        private void CreateCollectionTTLIndex<T>(IMongoCollection<T> collectionObject, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null)
        {
            if (stateContext != null && stateContext?.TTLExpiration != null && stateContext.TTLExpiration.TotalSeconds > 0 && !string.IsNullOrEmpty(stateContext.ExpirationFieldName))
            {
                try
                {
                   
                    var indexBuilder = Builders<T>.IndexKeys;
                    var keys = indexBuilder.Ascending(stateContext.ExpirationFieldName);
                    var options = new CreateIndexOptions
                    {
                        ExpireAfter = stateContext?.TTLExpiration
                    };
                    var indexModel = new CreateIndexModel<T>(keys, options);
                    var tsk = collectionObject.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
                   

                    tsk.Wait();
                }
                catch (AggregateException ex)
                {

                }
            }
        }

        #endregion

        #region CRUD Operations

        #region Create/Add Documents

        public StoreResult<T> InsertSingle<T>(T @object, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            GetCollection<T>(cancellationtoken, stateContext).InsertOne(@object);
            return new StoreResult<T>(true, @object,  Enumerable.Empty<T>());
        }

        public async Task<StoreResult<T>> InsertSingleAsync<T>(T @object, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            await GetCollection<T>(cancellationtoken, stateContext).InsertOneAsync(@object);
            return new StoreResult<T>(true, @object,  Enumerable.Empty<T>());
        }

        public StoreResult<T> InsertMany<T>(IEnumerable<T> objects, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            GetCollection<T>(cancellationtoken,stateContext).InsertMany(objects);
            return new StoreResult<T>(true, null, objects);
        }

        public async Task<StoreResult<T>> InsertManyAsync<T>(IEnumerable<T> objects, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            await GetCollection<T>(cancellationtoken, stateContext).InsertManyAsync(objects);
            return new StoreResult<T>(true, null, objects);
        }

        #endregion

        #region Read/Get Documents
        public T FindOne<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = GetCollection<T>(cancellationtoken, stateContext).Find<T>(filter).FirstOrDefault();
            return result;
        }

        public async Task<T> FindOneAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = await GetCollection<T>(cancellationtoken, stateContext).FindAsync<T>(filter);
            return result.FirstOrDefault();
        }

        public IEnumerable<T> FindAll<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            return GetCollection<T>(cancellationtoken, stateContext).Find<T>(filter).ToList();
        }

        public IEnumerable<T> FindAll<T>(Expression<Func<T, bool>> expression, FindOptions<T> options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            return GetCollection<T>(cancellationtoken, stateContext).FindSync<T>(filter, options).ToList();
        }

        public async Task<IEnumerable<T>> FindAllAysnc<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = await GetCollection<T>(cancellationtoken, stateContext).FindAsync<T>(filter).ConfigureAwait(false);
            //Task.WaitAny(result);
            return result.ToList();
        }

        public async Task<IEnumerable<T>> FindAllAysnc<T>(Expression<Func<T, bool>> expression, FindOptions<T> options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = await GetCollection<T>(cancellationtoken, stateContext).FindAsync<T>(filter, options).ConfigureAwait(true);

            // await Task.WhenAll(result);
            return result.ToList();
            //Task<IEnumerable<T>> t =await result;
            //return Task.FromResult(result);
        }


        public P FindAndProjectOne<T, P>(Expression<Func<T, P>> projectionParameter, Expression<Func<T, bool>> searchParameters, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var projection = Builders<T>.Projection.Expression(projectionParameter);
            var filter = Builders<T>.Filter.And(searchParameters);
            return GetCollection<T>(cancellationtoken, stateContext).Find(filter).Project(projection).FirstOrDefault();
        }

        public List<P> FindAndProjectMany<T, P>(Expression<Func<T, P>> projectionParameter, Expression<Func<T, bool>> searchParameters, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var projection = Builders<T>.Projection.Expression(projectionParameter);
            var filter = Builders<T>.Filter.And(searchParameters);
            return GetCollection<T>(cancellationtoken,stateContext).Find(filter).Project(projection).ToList();
        }

        public IEnumerable<T> FindUsingRegex<T>(string fieldName, string searchKey, RegexOptions options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var regex = new BsonRegularExpression(new Regex(searchKey, options));
            var filter = Builders<T>.Filter.Regex(fieldName, regex);
            var result = GetCollection<T>(cancellationtoken, stateContext).Find<T>(filter).ToList();
            return result;
        }

        public List<P> FindAndProjectEmbeddedDocuments<T, P>(BsonDocument[] pipeline, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = GetCollection<T>(cancellationtoken, stateContext).Aggregate<P>(pipeline).ToList();
            return result;
        }

        #endregion

        #region Update Documents

        public T FindOneAndUpdate<T, TField>(Expression<Func<T, TField>> expression, TField value, UpdateDefinition<T> updateDef, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.Eq(expression, value);
            var result = GetCollection<T>(cancellationtoken,stateContext).FindOneAndUpdate<T>(filter, updateDef);
            return result;
        }

        public T FindOneAndUpdate<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = GetCollection<T>(cancellationtoken,stateContext).FindOneAndUpdate<T>(filter, updateDef);
            return result;
        }

        public T FindOneAndUpdate<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, FindOneAndUpdateOptions<T> options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = GetCollection<T>(cancellationtoken,stateContext).FindOneAndUpdate<T>(filter, updateDef, options);
            return result;
        }

        public UpdateResult UpdateMany<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = GetCollection<T>(cancellationtoken,stateContext).UpdateMany<T>(expression, updateDef);
            return result;
        }


        public async Task<UpdateResult> UpdateManyAsync<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = await GetCollection<T>(cancellationtoken,stateContext).UpdateManyAsync<T>(expression, updateDef);
            return result;
        }

        public ReplaceOneResult ReplaceOne<T>(T @object, Expression<Func<T, bool>> key, ReplaceOptions options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {

            var filter = Builders<T>.Filter.And(key);
            var result = GetCollection<T>(cancellationtoken,stateContext).ReplaceOne(filter, @object, options);
            return result;
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync<T>(T @object, Expression<Func<T, bool>> key, ReplaceOptions options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(key);
            var result = await GetCollection<T>(cancellationtoken,stateContext).ReplaceOneAsync(filter, @object, options);
            return result;
        }

        #endregion

        #region Remove/Delete Documents

        public DeleteResult DeleteOne<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = GetCollection<T>(cancellationtoken,stateContext).DeleteOne<T>(expression);
            return result;
        }

        public async Task<DeleteResult> DeleteOneAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = await GetCollection<T>(cancellationtoken,stateContext).DeleteOneAsync<T>(expression);
            return result;
        }

        public DeleteResult DeleteMany<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = GetCollection<T>(cancellationtoken,stateContext).DeleteMany<T>(expression);
            return result;
        }

        public async Task<DeleteResult> DeleteManyAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var result = await GetCollection<T>(cancellationtoken,stateContext).DeleteManyAsync<T>(expression);
            return result;
        }

        public T FindOneAndDelete<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = GetCollection<T>(cancellationtoken,stateContext).FindOneAndDelete<T>(filter);
            return result;
        }

        public async Task<T> FindOneAndDeleteAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = await GetCollection<T>(cancellationtoken,stateContext).FindOneAndDeleteAsync<T>(filter);
            return result;
        }
        public T FindOneAndDelete<T>(Expression<Func<T, bool>> expression, FindOneAndDeleteOptions<T> options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = GetCollection<T>(cancellationtoken,stateContext).FindOneAndDelete<T>(filter, options);
            return result;
        }

        public async Task<T> FindOneAndDeleteAsync<T>(Expression<Func<T, bool>> expression, FindOneAndDeleteOptions<T> options, CancellationToken cancellationtoken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            var filter = Builders<T>.Filter.And(expression);
            var result = await GetCollection<T>(cancellationtoken,stateContext).FindOneAndDeleteAsync<T>(filter, options);
            return result;
        }

        #endregion       

        #endregion
    }
}
