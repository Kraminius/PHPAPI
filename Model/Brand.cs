using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Brand
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public Location[]? Locations { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public Ware[]? Wares { get; set; }

        public Brand(string name, Location[] locations, DateTime open, DateTime close, Ware[] wares)
        {
            Name = name;
            Locations = locations;
            OpenTime = open;
            CloseTime = close;
            Wares = wares;
        }
    }
}