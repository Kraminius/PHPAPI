using H3;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PHPAPI.Model
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }    
        public byte[]? Salt { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        
        [BsonElement("h3Index")]
        public string H3Index { get; set; }
        public Location[] Location { get; set; }
        public string Role { get; set; } = "User";

        public User() { }

        public User(string username, string email, string name, string h3Index, string password, Location[] location)
        {
            Id = ObjectId.GenerateNewId();
            Username = username;
            Email = email;
            Name = name;
            H3Index = h3Index;
            Role = "User";
            Salt = GenerateSalt();
            SetPassword(password, Salt);
            Location = location;
        }
        private void SetPassword(string password, byte[] salt)
        {         
            PasswordHash = Convert.ToBase64String(HashPassword(password, Salt));
        }

        private byte[] GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            return salt;
        }

        private byte[] HashPassword(string password, byte[] salt)
        {
            int iterations = 50000; //TODO: TEST PERFORMANCE, LOWER = QUICKER, HIGHER = SLOWER BUT MORE SECURE
            var hashAlgorithm = HashAlgorithmName.SHA256;

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, hashAlgorithm);
            return pbkdf2.GetBytes(20);
        }


        public bool VerifyPassword(string password)
        {
            byte[] hashOfInput = HashPassword(password, Salt);
            byte[] hashOfStoredPassword = Convert.FromBase64String(PasswordHash);
            return AreHashesEqual(hashOfInput, hashOfStoredPassword);
        }

        private bool AreHashesEqual(byte[] firstHash, byte[] secondHash)
        {
            uint diff = (uint)firstHash.Length ^ (uint)secondHash.Length;
            for (int i = 0; i < firstHash.Length && i < secondHash.Length; i++)
            {
                diff |= (uint)(firstHash[i] ^ secondHash[i]);
            }
            return diff == 0;
        }

        public string toString()
        {
            return "ID: " + Id + "Username: " + Username + " Email: " + Email + " Name: " + Name + " H3Index: " + H3Index + " Role: " + Role;
        }
    }
}
