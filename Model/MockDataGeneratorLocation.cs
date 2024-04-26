using MongoDB.Driver.GeoJsonObjectModel;

namespace PHPAPI.Model
{
    public class MockDataGeneratorLocation
    {

        private static Random _random = new Random();

        public static List<UserGeolocation> GenerateMockDataCopenhagen(int numberOfEntries)
        {
            var mockData = new List<UserGeolocation>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Copenhagen
                double latitude = 55.615 + _random.NextDouble() * (55.675 - 55.615);
                double longitude = 12.523 + _random.NextDouble() * (12.650 - 12.523);

                // Create a GeoJsonPoint for the Location
                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var mockGeolocation = new UserGeolocation
                {
                    UserId = $"mockUserId{i + 1}",
                    Location = location
                };

                mockData.Add(mockGeolocation);
            }

            return mockData;
        }

        public static List<UserGeolocation> GenerateMockDataForAarhus(int numberOfEntries)
        {
            var mockData = new List<UserGeolocation>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Aarhus
                double latitude = 56.120 + _random.NextDouble() * (56.180 - 56.120);
                double longitude = 10.180 + _random.NextDouble() * (10.250 - 10.180);

                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var mockGeolocation = new UserGeolocation
                {
                    UserId = $"AarhusUserId{i + 1}",
                    Location = location
                };

                mockData.Add(mockGeolocation);
            }

            return mockData;
        }

        public static List<UserGeolocation> GenerateMockDataForMøn(int numberOfEntries)
        {
            var mockData = new List<UserGeolocation>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Møn
                double latitude = 54.900 + _random.NextDouble() * (55.010 - 54.900);
                double longitude = 12.400 + _random.NextDouble() * (12.560 - 12.400);

                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var mockGeolocation = new UserGeolocation
                {
                    UserId = $"MonUserId{i + 1}",
                    Location = location
                };

                mockData.Add(mockGeolocation);
            }

            return mockData;
        }

        //Create Brands for mock stores
        private static List<Brand> GenerateMockBrands(int numberOfBrands, int waresInStore){
            var brands = new List<Brand>();

            for(int i = 0; i < numberOfBrands; i++)
            {
                //Wares for the Brand
                var wares = new List<Ware>();
                for (int j = 0; j < waresInStore; j++)
                {
                    var ware = new Ware(
                        name: $"mockWare{j + 1} of brand{i + 1}",
                        description: $"mockDescription for Ware{j + 1} of brand{i + 1}",
                        manufactorer: $"made by mockManufactorer{j}",
                        price: (float)(_random.NextDouble() * 10),
                        expiration: DateTime.UtcNow.AddDays(_random.Next(1, 365))
                    );

                    wares.Add(ware);
                }
                var brand = new Brand(
                    name: $"mockBrand {i}",
                    open: DateTime.UtcNow.AddDays(1).AddHours(-3),
                    close: DateTime.UtcNow.AddDays(1).AddHours(8),
                    wares: wares
                    );
                brands.Add(brand);
            }

            return brands;
           
        }


        //Create mock stores
        public static List<Store> GenerateMockStoresForCopenhagen(int numberOfStores, int numberOfBrands, int numberOfWares)
        {
            var mockData = new List<Store>();

            List<Brand> brands = GenerateMockBrands(numberOfBrands, numberOfWares);

            for(int i = 0; i < numberOfStores; i++)
            {

                //Brand to use
                var brandNum = (numberOfBrands % i);

                // Generate random coordinates within the specified range for Copenhagen
                double latitude = 55.615 + _random.NextDouble() * (55.675 - 55.615);
                double longitude = 12.523 + _random.NextDouble() * (12.650 - 12.523);

                // Create a GeoJsonPoint for the Location
                var newLocation = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var store = new Store(
                    brand: brands[brandNum],
                    location: newLocation
                    ) ;
                mockData.Add(store);
            }

            return mockData;
        }

        /*
        //Create mock stores
        public static List<BrandGeolocation> GenerateMockStoresForCopenhagen(int numberOfStores, int waresInStore)
        {
            var mockData = new List<BrandGeolocation>();

            for (int i = 0; i < numberOfStores; i++)
            {
                // Shift between generating mockNetto and mockBrugsen
                string brand;
                if (i % 2 == 0)
                    brand = "mockNetto";
                else
                    brand = "mockBrugsen";

                //Wares for the store (Might remove)
                var wares = new List<Ware>();
                for (int j = 0; j < waresInStore; j++)
                {
                    var ware = new Ware(
                        name: $"mockWare{j + 1} of {brand}",
                        description: $"mockDescription for Ware{j + 1} of {brand}",
                        manufactorer: brand,
                        price: (float)(_random.NextDouble() * 10),
                        expiration: DateTime.UtcNow.AddDays(_random.Next(1, 365))
                    );

                    wares.Add(ware);
                }

                    // Generate random coordinates within the specified range for Copenhagen
                    double latitude = 55.615 + _random.NextDouble() * (55.675 - 55.615);
                    double longitude = 12.523 + _random.NextDouble() * (12.650 - 12.523);

                    // Create a GeoJsonPoint for the Location
                    var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                    var mockGeolocation = new BrandGeolocation
                    {
                        LocationID = $"mockBrandId{i + 1}",
                        Location = location,
                        BrandId = brand,
                        Wares = wares //Might remove

                    };

                    mockData.Add(mockGeolocation);
                }

                return mockData;
            }
        */

    }
    }
