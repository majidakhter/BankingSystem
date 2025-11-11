using MongoDB.Bson.Serialization.Attributes;

namespace BankingAppDDD.Common.Mongo
{
    public class AggregationResponse<TResult> where TResult : class
    {
        [BsonElement("AggregationResponseObject")]
        public TResult? AggregationResponseObject { get; set; }
    }
}
