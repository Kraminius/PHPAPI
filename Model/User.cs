using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Salt { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string Role { get; set; }

        public User(string username, string password, string salt, string email, string name)
        {
            Username = username;
            Password = password;
            Salt = salt;
            Email = email;
            Name = name;
            Role = "User";
        }
    }
}