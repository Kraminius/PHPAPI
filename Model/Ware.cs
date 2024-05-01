using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Ware
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Manufactorer { get; set; }
        public float? Price { get; set; }
        public DateTime? Expiration {  get; set; }

        public Ware(string name, string description, string? manufactorer, float? price, DateTime? expiration)
        {
            Name = name;
            Description = description;
            Manufactorer = manufactorer;
            Price = price;
            Expiration = expiration;
        }
    }
}