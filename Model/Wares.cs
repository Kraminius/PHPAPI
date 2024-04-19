using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Wares
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? Weigh { get; set; }
        public int? Price { get; set; }
        public string? Brand { get; set; }

        public Wares(string name, DateTime expiration, int Weigh, int price, string? brand)
        {
            Name = name;
            ExpirationDate = expiration;
            Weigh = Weigh;
            Price = price;
        }
    }
}