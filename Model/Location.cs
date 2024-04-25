using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Location
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        // Location or perhaps a part of GeoLocation.cs

        public Location(string name)
        {
            Name = name;
        }
    }
}