using H3.Model;
using H3;
using MongoDB.Driver.GeoJsonObjectModel;

namespace PHPAPI.Model
{
    public class MockDataGeneratorStore
    {

        private static Random _random = new Random();

        public static List<Store> GenerateMockStoreCopenhagen(int numberOfEntries)
        {
            var mockData = new List<Store>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Copenhagen
                double latitude = 55.615 + _random.NextDouble() * (55.771 - 55.615);
                double longitude = 12.298 + _random.NextDouble() * (12.650 - 12.298);

                // Create a GeoJsonPoint for the Location
                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7);

                TimeSpan? expriryDate;

                if (i % 2 == 0)
                    expriryDate = new TimeSpan(i, 0, 0, 0);
                else
                    expriryDate = null;

                // Create mock wares
                var mockWares = new List<Ware>
                {
                    new Ware { Name = $"FirstProduct{i}", Price = 10 * i, Desc = "Description1", ImageUrl = "ImageUrl1", Amount = 1 },
                    new Ware { Name = $"SecondProduct{i}", Price = 2 * i, Desc = "Description2", ImageUrl = "ImageUrl2", Amount = 2 }
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
                    H3Index = h3Index.ToString(),
                    Brand = mockBrand
                };
                mockData.Add(mockStore);

            }
            return mockData;
        }

        public static List<Store> GenerateMockDataForAarhus(int numberOfEntries)
        {
            var mockData = new List<Store>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Aarhus
                double latitude = 56.120 + _random.NextDouble() * (56.180 - 56.120);
                double longitude = 10.180 + _random.NextDouble() * (10.250 - 10.180);

                // Create a GeoJsonPoint for the Location
                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7);

                TimeSpan? expriryDate;

                if (i % 2 == 0)
                    expriryDate = new TimeSpan(i, 0, 0, 0);
                else
                    expriryDate = null;

                // Create mock wares
                var mockWares = new List<Ware>
            {
                new Ware { Name = $"FirstProduct{i}", Price = 10 * i, Desc = "Description1", ImageUrl = "ImageUrl1", Amount = 1 },
                new Ware { Name = $"SecondProduct{i}", Price = 2 * i, Desc = "Description2", ImageUrl = "ImageUrl2", Amount = 2 }
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
                    H3Index = h3Index.ToString(),
                    Brand = mockBrand
                };
                mockData.Add(mockStore);

            }
            return mockData;
        }

        public static List<Store> GenerateMockDataForMon(int numberOfEntries)
        {
            var mockData = new List<Store>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Møn
                double latitude = 54.900 + _random.NextDouble() * (55.010 - 54.900);
                double longitude = 12.100 + _random.NextDouble() * (12.560 - 12.100);

                // Create a GeoJsonPoint for the Location
                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7);

                TimeSpan? expriryDate;

                if (i % 2 == 0)
                    expriryDate = new TimeSpan(i, 0, 0, 0);
                else
                    expriryDate = null;

                // Create mock wares
                var mockWares = new List<Ware>
            {
                new Ware { Name = $"FirstProduct{i}", Price = 10 * i, Desc = "Description1", ImageUrl = "ImageUrl1", Amount = 1 },
                new Ware { Name = $"SecondProduct{i}", Price = 2 * i, Desc = "Description2", ImageUrl = "ImageUrl2", Amount = 2 }
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
                    H3Index = h3Index.ToString(),
                    Brand = mockBrand
                };
                mockData.Add(mockStore);

            }
            return mockData;
        }

        

    }
}
