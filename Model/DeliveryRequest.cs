using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.ComponentModel.DataAnnotations;

namespace PHPAPI.Model
{
    public class DeliveryRequest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public ObjectId Id { get; set; }
        [Required]
        public User RequestUser { get; set; }
        [Required]
        public User HelpUser { get; set; }

        [Required]
        public Ware[] Wares { get; set; }

        [Required]
        public StateOfRequest.Status State { get; set; }

        [Required]
        public Location Location { get; set; }
        [Required]
        public string H3Index { get; set; }

        [Required]
        public string TimeOfRequest { get; set; }

        public DeliveryRequest() { }

        public class StateOfRequest
        {
            public enum Status
            {
                REQUESTED,
                ONGOING,
                DONE
            }
        }
    }
}