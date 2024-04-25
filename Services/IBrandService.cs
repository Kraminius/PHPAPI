using PHPAPI.Model;

namespace PHPAPI.Services
{
    public interface IBrandService
    {
        Task CreateBrand(Model.Brand brand);
        Task<Model.Brand> GetBrandByName(string name);
    }
}
