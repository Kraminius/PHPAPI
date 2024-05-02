using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PHPAPI.Model
{
    public class Brand
    {


        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("openTime")]
        public DateTime? OpenTime { get; set; }

        [BsonElement("closeTime")]
        public DateTime? CloseTime { get; set; }

        [BsonElement("wares")]
        public List<Ware> Wares { get; set; }

    }
}
