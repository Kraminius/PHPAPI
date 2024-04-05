namespace PeopleHelpPeople.Model
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string GeolocationCollectionName { get; set; } = null!;
    } 
}
