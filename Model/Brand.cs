using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Brand
    {
        public ObjectId Id { get; set; }
        public Wares[]? Wares { get; set; }
        public string? Name {  get; set; }

        public Brand(string name, Wares[] wares)
        {
            Name = name;
            Wares = wares;
        }
    }
}