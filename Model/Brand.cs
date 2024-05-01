using Microsoft.AspNetCore.SignalR.Protocol;
using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Brand
    {
        public ObjectId Id { get; set; }
        public string? Name { get; set; }
        public DateTime? OpenTime { get; set; }
        public DateTime? CloseTime { get; set; }
        public List<Ware>? Wares { get; set; }
        // Location or perhaps a part of GeoLocation.cs

        public Brand(string name, DateTime close, DateTime open, List<Ware> wares)
        {
            Name = name;
            OpenTime = open;
            CloseTime = close;
            Wares = wares;
        }
    }
}