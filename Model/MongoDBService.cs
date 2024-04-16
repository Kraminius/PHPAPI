using MongoDB.Driver;
using System.Threading.Tasks;
using PHPAPI.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson;

namespace PHPAPI.Model
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

            CreateGeospatialIndex();
        }

        private void CreateGeospatialIndex()
        {
            var indexKeysDefinition = Builders<UserGeolocation>.IndexKeys.Geo2DSphere(x => x.Location);
            _geolocations.Indexes.CreateOne(new CreateIndexModel<UserGeolocation>(indexKeysDefinition));
        }

        public async Task InsertGeolocationAsync(UserGeolocation geolocation)
        {
            await _geolocations.InsertOneAsync(geolocation);
        }

        public async Task InsertMockDataIfNeededAsync()
        {
            try
            {
                var mockGeolocations = new List<UserGeolocation>();

                for (int i = 1; i <= 10; i++)
                {
                    // Create a GeoJsonPoint for the Location
                    var location = GeoJson.Point(GeoJson.Geographic(8 + i, 55)); // Very important GeoJson use (longitude, latitude) and Google map use (latitude, longitude)

                    var mockGeolocation = new UserGeolocation
                    {
                        UserId = $"mockUserId{i}",
                        Location = location 
                    };

                    mockGeolocations.Add(mockGeolocation);
                }

                // Insert all mock geolocations into the database
                await _geolocations.InsertManyAsync(mockGeolocations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting mock data: {ex.Message}");
            }
        }

        public async Task<UserGeolocation> FindNearestAsync(double latitude, double longitude, int meters)
        {
            var point = GeoJson.Point(GeoJson.Geographic(longitude, latitude)); // Very important GeoJson use (longitude, latitude) and Google map use (latitude, longitude)
            var filter = Builders<UserGeolocation>.Filter.NearSphere(x => x.Location, point, maxDistance: meters);
            Console.WriteLine(_geolocations.Find(filter));

            return await _geolocations.Find(filter).FirstOrDefaultAsync();
        }

        // Here it is possible to also get everyones distances in a sorted list, can be used for optimization and more information
        /*public async Task<UserGeolocation> FindNearestAsync(double latitude, double longitude, int maxDistanceInMeters)
        {
            var geoNearStage = new BsonDocument("$geoNear", new BsonDocument
            {
            { "near", new BsonDocument { { "type", "Point" }, { "coordinates", new BsonArray { latitude  , longitude } } } },
            { "distanceField", "distance" },
            { "maxDistance", maxDistanceInMeters },
            { "spherical", true }
            });

            var pipeline = PipelineDefinition<UserGeolocation, UserGeolocation>.Create(new[] { geoNearStage });
            var results = await _geolocations.Aggregate(pipeline).ToListAsync();

            // Test printing
            foreach (var user in results)
            {
                Console.WriteLine($"User: {user.UserId}, Distance: {user.distance}");
            }

            
            return results.FirstOrDefault();
        }*/

    }
}