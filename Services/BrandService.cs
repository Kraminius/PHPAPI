using MongoDB.Driver;

namespace PHPAPI.Services
{
    public class BrandService : IBrandService
    {
        private readonly IMongoCollection<Model.Brand> brandCollection;

        public BrandService(IMongoDBContext context)
        {
            brandCollection = context.GetCollection<Model.Brand>("Brands");
        }

        public async Task CreateBrand(Model.Brand brand)
        {
            await brandCollection.InsertOneAsync(brand);
        }

        public async Task<Model.Brand> GetBrandByName(string name)
        {
            return await brandCollection.Find(u => u.Name == name).FirstOrDefaultAsync();
        }
    }
}
