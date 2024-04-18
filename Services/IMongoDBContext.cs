using MongoDB.Driver;

namespace PHPAPI.Services
{
    public interface IMongoDBContext
    {
        IMongoCollection<T> GetCollection<T>(string name);

        Task InsertOneAsync<T>(T document);
        Task<IEnumerable<T>> FindAsync<T>(FilterDefinition<T> filter);
        Task<DeleteResult> DeleteOneAsync<T>(FilterDefinition<T> filter);
    }
}