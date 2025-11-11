using System.Collections.Generic;

namespace BankingAppDDD.Common.Mongo.Helper
{
    public class StoreResult<T>
    {
        public StoreResult(bool success, T document, IEnumerable<T> documents)
        {
            Success = success;
            Document = document;
            Documents = documents;       
        }
        public T Document { get; set; }  
        public IEnumerable<T> Documents { get; set; }
        public bool Success { get; set; }       
    }
}
