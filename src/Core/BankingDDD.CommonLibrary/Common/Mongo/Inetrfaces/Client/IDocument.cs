using BankingAppDDD.Common.Mongo.Helper;
using BankingAppDDD.Common.Mongo.Interfaces.Collection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Text.RegularExpressions;


namespace BankingAppDDD.Common.Mongo.Interfaces.Client
{
    public interface IDocument
    {
        #region Create/Add Document(s)

        StoreResult<T> AddOne<T>(T @object, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<StoreResult<T>> AddOneAsync<T>(T @object, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        StoreResult<T> AddMany<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<StoreResult<T>> AddManyAsync<T>(IEnumerable<T> objects, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;

        #endregion

        #region Read/Get Document(s)
        T GetOne<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<T> GetOneAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        IEnumerable<T> GetAll<T>(Expression<Func<T, bool>> expression, FindOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<IEnumerable<T>> GetAllAysnc<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<IEnumerable<T>> GetAllAysnc<T>(Expression<Func<T, bool>> expression, FindOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        P GetAndProjectOne<T, P>(Expression<Func<T, P>> projectionParameter, Expression<Func<T, bool>> searchParameters, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        List<P> GetAndProjectMany<T, P>(Expression<Func<T, P>> projectionParameter, Expression<Func<T, bool>> searchParameters, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        IEnumerable<T> GetUsingRegex<T>(string fieldName, string searchKey, RegexOptions options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        List<P> GetAndProjectEmbeddedDocuments<T, P>(BsonDocument[] pipeline, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;

        #endregion

        #region Update Document(s)
        T FindOneAndUpdate<T, TField>(Expression<Func<T, TField>> expression, TField value, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        T FindOneAndUpdate<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        T FindOneAndUpdate<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, FindOneAndUpdateOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        UpdateResult UpdateMany<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<UpdateResult> UpdateManyAsync<T>(Expression<Func<T, bool>> expression, UpdateDefinition<T> updateDef, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        ReplaceOneResult ReplaceOne<T>(T @object, Expression<Func<T, bool>> expression, ReplaceOptions options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<ReplaceOneResult> ReplaceOneAsync<T>(T @object, Expression<Func<T, bool>> expression, ReplaceOptions options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        #endregion

        #region Remove/Delete Documents
        DeleteResult DeleteOne<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<DeleteResult> DeleteOneAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        DeleteResult DeleteMany<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<DeleteResult> DeleteManyAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        T FindOneAndDelete<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<T> FindOneAndDeleteAsync<T>(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        T FindOneAndDelete<T>(Expression<Func<T, bool>> expression, FindOneAndDeleteOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;
        Task<T> FindOneAndDeleteAsync<T>(Expression<Func<T, bool>> expression, FindOneAndDeleteOptions<T> options, CancellationToken cancellationToken = default, IMongoDBStateContext? stateContext = null) where T : class;

        #endregion
    }
}
