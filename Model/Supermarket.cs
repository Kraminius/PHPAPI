using MongoDB.Bson;

namespace PHPAPI.Model
{
    public class Supermarket
    {
        public ObjectId Id { get; set; }
        public Brand? Brand { get; set; }




        public Supermarket(Brand brand)
        {
           Brand = brand;
        }
    }
}