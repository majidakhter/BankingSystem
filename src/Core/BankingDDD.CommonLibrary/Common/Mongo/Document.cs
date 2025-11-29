using BankingAppDDD.Common.Mongo.Helper;
using BankingAppDDD.Common.Mongo.Interfaces.Client;
using BankingAppDDD.Common.Mongo.Interfaces.Collection;
using BankingAppDDD.Common.Mongo.Interfaces.Operations;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace BankingAppDDD.Common.Mongo
{
    public class Document : IDocument
    {
        private readonly IMongoDbContext context;

        public Document(IMongoDbContext contextDefault)
        {
            context = contextDefault;
        }

        #region Create/Add Document(s)

        public StoreResult<T> AddOne<T>(T @object, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.InsertSingle<T>(@object, cancellationToken, stateContext);
        }

        public async Task<StoreResult<T>> AddOneAsync<T>(T @object, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.InsertSingleAsync<T>(@object, cancellationToken, stateContext);
        }

        public StoreResult<T> AddMany<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.InsertMany(objects, cancellationToken, stateContext);
        }

        public async Task<StoreResult<T>> AddManyAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.InsertManyAsync(objects, cancellationToken, stateContext);
        }

        #endregion

        #region Read/Get Document(s)
        public T GetOne<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindOne<T>(expression, cancellationToken, stateContext);
        }

        public async Task<T> GetOneAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.FindOneAsync<T>(expression, cancellationToken, stateContext);
        }

        public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindAll<T>(expression, cancellationToken, stateContext);
        }

        public IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> expression, FindOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindAll(expression, options, cancellationToken, stateContext);
        }

        public async Task<IEnumerable<T>> GetAllAysnc<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.FindAllAysnc<T>(expression, cancellationToken, stateContext);
        }

        public async Task<IEnumerable<T>> GetAllAysnc<T>(Expression<Func<T, bool>> expression, FindOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.FindAllAysnc<T>(expression, options, cancellationToken, stateContext);
        }


        public P GetAndProjectOne<T, P>(Expression<Func<T, P>> projectionParameter, Expression<Func<T, bool>> searchParameters, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindAndProjectOne<T, P>(projectionParameter, searchParameters, cancellationToken, stateContext);
        }

        public List<P> GetAndProjectMany<T, P>(Expression<Func<T, P>> projectionParameter, Expression<Func<T, bool>> searchParameters, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindAndProjectMany<T, P>(projectionParameter, searchParameters, cancellationToken, stateContext);
        }

        public IEnumerable<T> GetUsingRegex<T>(string fieldName, string searchKey, RegexOptions options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindUsingRegex<T>(fieldName, searchKey, options, cancellationToken, stateContext);
        }

        public List<P> GetAndProjectEmbeddedDocuments<T, P>(BsonDocument[] pipeline, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindAndProjectEmbeddedDocuments<T, P>(pipeline, cancellationToken, stateContext);
        }

        #endregion

        #region Update Document(s)

        public T FindOneAndUpdate<T, TField>(Expression<Func<T, TField>> expression, TField value, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindOneAndUpdate<T, TField>(expression, value, updateDef, cancellationToken, stateContext);
        }

        public T FindOneAndUpdate<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindOneAndUpdate<T>(expression, updateDef, cancellationToken, stateContext);
        }

        public T FindOneAndUpdate<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, FindOneAndUpdateOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindOneAndUpdate<T>(expression, updateDef, options, cancellationToken, stateContext);
        }

        public UpdateResult UpdateMany<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.UpdateMany<T>(expression, updateDef, cancellationToken, stateContext);
        }


        public async Task<UpdateResult> UpdateManyAsync<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.UpdateManyAsync<T>(expression, updateDef, cancellationToken, stateContext);
        }

        public ReplaceOneResult ReplaceOne<T>(T @object, Expression<Func<T, bool>> expression, ReplaceOptions options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.ReplaceOne<T>(@object, expression, options, cancellationToken, stateContext);
        }

        public async Task<ReplaceOneResult> ReplaceOneAsync<T>(T @object, Expression<Func<T, bool>> expression, ReplaceOptions options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.ReplaceOneAsync<T>(@object, expression, options, cancellationToken, stateContext);
        }

        #endregion

        #region Remove/Delete Document(s)

        public DeleteResult DeleteOne<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.DeleteOne<T>(expression, cancellationToken, stateContext);
        }

        public async Task<DeleteResult> DeleteOneAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.DeleteOneAsync<T>(expression, cancellationToken, stateContext);
        }

        public DeleteResult DeleteMany<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.DeleteMany<T>(expression, cancellationToken, stateContext);
        }

        public async Task<DeleteResult> DeleteManyAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.DeleteManyAsync<T>(expression, cancellationToken, stateContext);
        }

        public T FindOneAndDelete<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindOneAndDelete<T>(expression, cancellationToken, stateContext);
        }

        public async Task<T> FindOneAndDeleteAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.FindOneAndDeleteAsync<T>(expression, cancellationToken, stateContext);
        }
        public T FindOneAndDelete<T>(Expression<Func<T, bool>> expression, FindOneAndDeleteOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return context.FindOneAndDelete<T>(expression, options, cancellationToken, stateContext);
        }

        public async Task<T> FindOneAndDeleteAsync<T>(Expression<Func<T, bool>> expression, FindOneAndDeleteOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class
        {
            return await context.FindOneAndDeleteAsync<T>(expression, options, cancellationToken, stateContext);
        }

        #endregion     

    }
}
