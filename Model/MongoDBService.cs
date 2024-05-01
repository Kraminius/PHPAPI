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
        public IMongoCollection<User> Users { get; private set; }

        public MongoDBService(IOptions<MongoDBSettings> settings)
        {


            Console.WriteLine("WHATUP");
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _geolocations = database.GetCollection<UserGeolocation>(settings.Value.GeolocationCollectionName);
            Users = database.GetCollection<User>(settings.Value.UserCollectionName);

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


        public async Task InsertUserAsync(User user)
        {
            await Users.InsertOneAsync(user);
        }

        public async Task<IEnumerable<User>> FindUsersAsync(FilterDefinition<User> filter)
        {
            var results = await Users.FindAsync(filter);
            return results.ToEnumerable();
        }

        public async Task<DeleteResult> DeleteUserAsync(FilterDefinition<User> filter)
        {
            return await Users.DeleteOneAsync(filter);
        }

    }


    //New Brands
    public class MongoStoreDBService
    {
        private readonly IMongoCollection<Store> store;

        public MongoStoreDBService(IOptions<MongoDBSettings> settings)
        {

            Console.WriteLine("NOT MUCH, YOU?");
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            store = database.GetCollection<Store>(settings.Value.GeolocationCollectionName);

            CreateGeospatialIndex();
        }

        private void CreateGeospatialIndex()
        {
            var indexKeysDefinition = Builders<Store>.IndexKeys.Geo2DSphere(x => x.Location);
            store.Indexes.CreateOne(new CreateIndexModel<Store>(indexKeysDefinition));
        }

        public async Task InsertGeolocationAsync(Store geolocation)
        {
            await store.InsertOneAsync(geolocation);
        }

        public async Task DeleteAllGeolocationsAsync()
        {
            // This will delete all documents from the Store collection
            await store.DeleteManyAsync(Builders<Store>.Filter.Empty);
        }

        public async Task<List<Store>> GetAllGeolocationsAsync()
        {
            return await store.Find(_ => true).ToListAsync();
        }

        public async Task InsertMockDataIfNeededAsync()
        {
            try
            {
                // Insert all mock geolocations into the database
                await store.InsertManyAsync(MockDataGeneratorLocation.GenerateMockStoresForCopenhagen(10, 2, 8));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting mock data: {ex.Message}");
            }
        }

        public async Task<Store> FindNearestAsync(double latitude, double longitude, int meters, string brand)
        {

            var point = GeoJson.Point(GeoJson.Geographic(longitude, latitude)); // Very important GeoJson use (longitude, latitude) and Google map use (latitude, longitude)
            var locationFilter = Builders<Store>.Filter.NearSphere(x => x.Location, point, maxDistance: meters); //Stores close by
            //var brandFilter = Builders<Store>.Filter.Eq((x => x.Brand.name, brand); //Stores with mathing brand
            var brandFilter = Builders<Store>.Filter.Eq(x => x.Brand.Name, brand);

            var combinedFilter = Builders<Store>.Filter.And(locationFilter, brandFilter);

            Console.WriteLine(store.Find(combinedFilter).FirstOrDefaultAsync());

            return await store.Find(combinedFilter).FirstOrDefaultAsync();
        }
        public async Task<List<Store>> FindGeolocationsByH3IndexAsync(string h3Index)
        {
            var filter = Builders<Store>.Filter.Eq(geo => geo.H3Index, h3Index);
            return await store.Find(filter).ToListAsync();
        }


    }
}