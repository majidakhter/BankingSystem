namespace BankingAppDDD.Identity.Model
{
    public class MongoDbSetting
    {
        public string ConnectionString { get; set; }
        public string DataBaseName { get; set; }
        public string DataBaseVersion { get; set; }
        public MongoCollection Collections { get; set; }

    }
    public class MongoCollection
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
