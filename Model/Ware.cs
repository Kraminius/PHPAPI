using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PHPAPI.Model
{
    public class Ware
    {


        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("producer")]
        public string Producer { get; set; }

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("expirationTime")]
        public TimeSpan? expirationTime { get; set; }

    }
}
