using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace PHPAPI.Model
{
    public class UserGeolocation
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates>? Location { get; set; }

        public double distance { get; set; }

    }


    // New for shops
    public class BrandGeolocation
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("locationId")]
        public string? LocationID { get; set; }
        [BsonElement("brandId")]
        public string? BrandId { get; set; }

        [BsonElement("latitude")]
        public double Latitude { get; set; }

        [BsonElement("longitude")]
        public double Longitude { get; set; }

        public GeoJsonPoint<GeoJson2DGeographicCoordinates>? Location { get; set; }

        public double distance { get; set; }

    }
}