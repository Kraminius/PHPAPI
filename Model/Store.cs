using MongoDB.Bson;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PHPAPI.Model
{
    public class Store
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public Brand Brand { get; set; }
        public byte[]? Salt { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Role { get; set; } = "Store";

        public Store() { }

        public Store(string name, Brand brand, string mail, string address)
        {
            Name = name;
            Brand = brand;
            Email = mail;
            Role = "User";
            Salt = GenerateSalt();
            Address = address;
            
        }

        private byte[] GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }
    }
}
