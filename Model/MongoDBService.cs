using MongoDB.Driver;
using System.Threading.Tasks;
using PeopleHelpPeople.Model;
using Microsoft.Extensions.Options;

namespace PeopleHelpPeople.Model // Replace 'YourProjectName' with the actual name of your project or a logical grouping for services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<UserGeolocation> _geolocations;

        public MongoDBService(IOptions<MongoDBSettings> settings)
        {

            Console.WriteLine("WHATUP");
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _geolocations = database.GetCollection<UserGeolocation>(settings.Value.GeolocationCollectionName);
        }

        public async Task InsertGeolocationAsync(UserGeolocation geolocation)
        {
            await _geolocations.InsertOneAsync(geolocation);
        }

        public async Task InsertMockDataIfNeededAsync()
        {
            try
            {
                // Insert mock data
                var mockGeolocation = new UserGeolocation
                {
                    UserId = "mockUserId",
                    Latitude = 40.7128,
                    Longitude = -74.0060
                };

                Console.WriteLine("hmmmm");

                await InsertGeolocationAsync(mockGeolocation);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting mock data: {ex.Message}");
                // Consider using a logging framework here
            }
        }
    }
}