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

        public string RequestIdKey { get; set; }

        public User RequestUser { get; set; }
        public User HelpUser {  get; set; }
        public Ware[] Wares { get; set; }
        public StateOfRequest.Status State {  get; set; }
        public Location Location { get; set; }
        public string H3Index { get; set; }


        public class StateOfRequest
        {
            public enum Status
            {
                REQUESTED,
                ONGOING,
                DONE
            }
        }

        //public double DeliveryLocationLongitude { get; set; }
        //public double DeliveryLocationLatitude { get; set; }
    }
}