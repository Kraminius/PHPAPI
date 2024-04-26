using MongoDB.Bson;
using System;
using System.Security.Cryptography;
using System.Text;

namespace PHPAPI.Model
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public byte[] Salt { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? HomeAddress { get; set; }
        public string? WorkAddress { get; set; }
        public string Role { get; set; } = "User";

        public User() { }

        public User(string username, string email, string name, string password, string homeAddress, String workAdress)
        {
            Username = username;
            Email = email;
            Name = name;
            Role = "User";
            Salt = GenerateSalt();
            SetPassword(password, Salt);
            HomeAddress = homeAddress;
            WorkAddress = workAdress;
            
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
    }
}
