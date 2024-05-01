namespace PHPAPI.Model
{
    public class MongoDBStoreSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string GeolocationCollectionName { get; set; } = null!;

        public string StoreCollectionName { get; set; }
    } 
}
