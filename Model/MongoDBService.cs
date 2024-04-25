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

    //New Brands
    public class MongoBrandDBService
    {
        private readonly IMongoCollection<BrandGeolocation> brandGeolocations;

        public MongoBrandDBService(IOptions<MongoDBSettings> settings)
        {

            Console.WriteLine("NOT MUCH, YOU?");
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            brandGeolocations = database.GetCollection<BrandGeolocation>(settings.Value.GeolocationCollectionName);

            CreateGeospatialIndex();
        }

        private void CreateGeospatialIndex()
        {
            var indexKeysDefinition = Builders<BrandGeolocation>.IndexKeys.Geo2DSphere(x => x.Location);
            brandGeolocations.Indexes.CreateOne(new CreateIndexModel<BrandGeolocation>(indexKeysDefinition));
        }

        public async Task InsertGeolocationAsync(BrandGeolocation geolocation)
        {
            await brandGeolocations.InsertOneAsync(geolocation);
        }

        public async Task DeleteAllGeolocationsAsync()
        {
            // This will delete all documents from the brandGeolocations collection
            await brandGeolocations.DeleteManyAsync(Builders<BrandGeolocation>.Filter.Empty);
        }

        public async Task<List<BrandGeolocation>> GetAllGeolocationsAsync()
        {
            return await brandGeolocations.Find(_ => true).ToListAsync();
        }

        public async Task InsertMockDataIfNeededAsync()
        {
            try
            {
                var mockGeolocations = new List<BrandGeolocation>();

                for (int i = 1; i <= 10; i++)
                {
                    // Create a GeoJsonPoint for the Location
                    var location = GeoJson.Point(GeoJson.Geographic(10 + i, 54)); // Very important GeoJson use (longitude, latitude) and Google map use (latitude, longitude)

                    string brand;
                    if (i % 2 == 0)
                        brand = "mockNetto";
                    else
                        brand = "mockBrugsen";

                    var mockGeolocation = new BrandGeolocation
                    {
                        LocationID = $"mockLocationId{i}",
                        Location = location,
                        BrandId = brand
                    };

                    mockGeolocations.Add(mockGeolocation);
                }

                // Insert all mock geolocations into the database
                await brandGeolocations.InsertManyAsync(mockGeolocations);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting mock data: {ex.Message}");
            }
        }

        public async Task<BrandGeolocation> FindNearestAsync(double latitude, double longitude, int meters, string brand)
        {

            var point = GeoJson.Point(GeoJson.Geographic(longitude, latitude)); // Very important GeoJson use (longitude, latitude) and Google map use (latitude, longitude)
            var locationFilter = Builders<BrandGeolocation>.Filter.NearSphere(x => x.Location, point, maxDistance: meters);
            var brandFilter = Builders<BrandGeolocation>.Filter.Eq(x => x.BrandId, brand);

            var combinedFilter = Builders<BrandGeolocation>.Filter.And(locationFilter, brandFilter);

            Console.WriteLine(brandGeolocations.Find(combinedFilter).FirstOrDefaultAsync());

            return await brandGeolocations.Find(combinedFilter).FirstOrDefaultAsync();
        }
        

    }



}