namespace BankingAppDDD.MongoService.Mongo.Model
{
    public class CountryReadModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
