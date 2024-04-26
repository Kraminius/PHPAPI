using MongoDB.Driver;
using System.Threading.Tasks;
using PHPAPI.Model;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System;
using MongoDB.Driver.GeoJsonObjectModel;
using MongoDB.Bson;
using H3.Model;
using H3;

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

        public async Task InsertManyGeolocationAsync(List<UserGeolocation> geolocations)
        {
            await _geolocations.InsertManyAsync(geolocations);
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

        public async Task DeleteAllGeolocationsAsync()
        {
            // This will delete all documents from the _geolocations collection
            await _geolocations.DeleteManyAsync(Builders<UserGeolocation>.Filter.Empty);
        }

        public async Task<List<UserGeolocation>> GetAllGeolocationsAsync()
        {
            return await _geolocations.Find(_ => true).ToListAsync();
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

        public async Task<List<UserGeolocation>> FindGeolocationsByH3IndexAsync(string h3Index)
        {
            var filter = Builders<UserGeolocation>.Filter.Eq(geo => geo.H3Index, h3Index);
            return await _geolocations.Find(filter).ToListAsync();
        }

    }
}