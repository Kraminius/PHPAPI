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
        //Store
        private readonly IMongoCollection<Store> stores;
        public IMongoCollection<User> Users { get; private set; }

        public MongoDBService(IOptions<MongoDBSettings> settings)
        {


            Console.WriteLine("WHATUP");
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _geolocations = database.GetCollection<UserGeolocation>(settings.Value.GeolocationCollectionName);
            //Store
            stores = database.GetCollection<Store>("stores");
            Users = database.GetCollection<User>(settings.Value.UserCollectionName);

            CreateGeospatialIndex();
        }

        public async Task InsertManyGeolocationAsync(List<UserGeolocation> geolocations)
        {
            await _geolocations.InsertManyAsync(geolocations);
        }

        //Store
        public async Task InsertManyStoresAsync(List<Store> storesInsert)
        {
            await stores.InsertManyAsync(storesInsert);
        }

        private void CreateGeospatialIndex()
        {
            var indexKeysDefinition = Builders<UserGeolocation>.IndexKeys.Geo2DSphere(x => x.Location);
            _geolocations.Indexes.CreateOne(new CreateIndexModel<UserGeolocation>(indexKeysDefinition));
        }

        //Store
        private void CreateGeospatialIndexStore()
        {
            var indexKeysDefinition = Builders<Store>.IndexKeys.Geo2DSphere(x => x.Location);
            stores.Indexes.CreateOne(new CreateIndexModel<Store>(indexKeysDefinition));
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

        //Store
        public async Task DeleteAllStoresAsync()
        {
            // This will delete all documents from the _geolocations collection
            await stores.DeleteManyAsync(Builders<Store>.Filter.Empty);
        }

        public async Task<List<UserGeolocation>> GetAllGeolocationsAsync()
        {
            return await _geolocations.Find(_ => true).ToListAsync();
        }

        //Store
        public async Task<List<Store>> GetAllStoresAsync()
        {
            return await stores.Find(_ => true).ToListAsync();
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

        //Store
        public async Task<List<Store>> FindNearestStoreAsync(double latitude, double longitude, int meters, string brandName)
        {
            var point = GeoJson.Point(GeoJson.Geographic(longitude, latitude)); // Very important GeoJson use (longitude, latitude) and Google map use (latitude, longitude)
            var filter1 = Builders<Store>.Filter.NearSphere(x => x.Location, point, maxDistance: meters);
            var filter2 = Builders<Store>.Filter.Eq("brand.name", brandName);
            var finalFilter = Builders<Store>.Filter.And(filter1, filter2);

            return await stores.Find(finalFilter).ToListAsync();
        }

        //Store
        public async Task<List<Store>> FindStoreByH3IndexAsync(string h3Index, string brandName)
        {
            var filter1 = Builders<Store>.Filter.Eq(geo => geo.H3Index, h3Index);
            var filter2 = Builders<Store>.Filter.Eq("brand.name", brandName);
            var finalFilter = Builders<Store>.Filter.And(filter1, filter2);
            return await stores.Find(finalFilter).ToListAsync();
        }


        public async Task InsertUserAsync(User user)
        {
            await Users.InsertOneAsync(user);
        }

        //Store
        public async Task InsertStoreAsync(Store store)
        {
            await stores.InsertOneAsync(store);
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


        //Store
        public async Task InsertMockStoresAsync()
        {
            try
            {
                var mockStores = new List<Store>();

                for (int i = 1; i <= 10; i++)
                {
                    // Create a GeoJsonPoint for the Location
                    var location = GeoJson.Point(GeoJson.Geographic(10 + i, 55 + i % 2));

                    TimeSpan? expriryDate;

                    if (i % 2 == 0)
                        expriryDate = new TimeSpan(i, 0, 0, 0);
                    else
                        expriryDate = null;

                    // Create mock wares
                    var mockWares = new List<Ware>
            {
                new Ware { Name = $"FirstProduct{i}", Producer = $"FirstProducer{i}", Price = 10 * i, ExpirationTime = expriryDate },
                new Ware { Name = $"SecondProduct{i}", Producer = $"SecondProducer{i}", Price = 2 * i, ExpirationTime = null }
            };

                    // Create a mock brand
                    var mockBrand = new Brand
                    {
                        Name = $"Brand{i}",
                        OpenTime = "08:00",
                        CloseTime = "21:00",
                        Wares = mockWares
                    };

                    var mockStore = new Store
                    {
                        Location = location,
                        Brand = mockBrand
                    };

                    mockStores.Add(mockStore);
                }

                // Insert all mock stores into the database
                await stores.InsertManyAsync(mockStores);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting mock data: {ex.Message}");
            }
        }

    }
}