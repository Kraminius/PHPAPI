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

        [BsonElement("desc")]
        public string Desc { get; set; }

        [BsonElement("price")]
        public double Price { get; set; }

        [BsonElement("imageUrl")]
        public string ImageUrl { get; set; }

        [BsonElement("amount")]
        public int Amount { get; set; }
        

    }
}
