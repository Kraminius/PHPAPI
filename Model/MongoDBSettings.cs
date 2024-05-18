namespace PHPAPI.Model
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string GeolocationCollectionName { get; set; } = null!;

        public string UserCollectionName { get; set; }

        public string RequestCollectionName { get; set; }
    } 
}
