using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace PHPAPI.Model
{
    public class Store
    {
        public ObjectId Id { get; set; }
        public Brand? Brand { get; set; }

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates>? Location { get; set; }

        public double distance { get; set; }

        public Store(Brand brand, GeoJsonPoint<GeoJson2DGeographicCoordinates>? location)
        {
            Brand = brand;
            Location = location;
        }
    }
}