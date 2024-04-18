using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace PHPAPI.Service
{
        public static class PasswordHasher
        {
            public static string GenerateSalt()
            {
                // Generate a secure random salt
                byte[] saltBytes = new byte[32];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                return Convert.ToBase64String(saltBytes);
            }

            public static string Hash(string password, string salt)
            {
                return BCrypt.Net.BCrypt.HashPassword(password, salt);
            }
        }

    }
