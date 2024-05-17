using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace PHPAPI.Model
{
    public class DeliveryRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string RequestId { get; set; }
        public string HelperId { get; set; }
        public string Item { get; set; }
        public string Status { get; set; }
        public double DeliveryLocationLongitude { get; set; }
        public double DeliveryLocationLatitude { get; set; }
        public string H3Index { get; set; }
    }
}