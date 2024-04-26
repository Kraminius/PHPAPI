using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace PHPAPI.Model
{
    public class UserGeolocation
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string UserId { get; set; }

        [BsonElement("location")]
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> Location { get; set; }

        [BsonElement("h3Index")]
        public string H3Index { get; set; }

    }
}