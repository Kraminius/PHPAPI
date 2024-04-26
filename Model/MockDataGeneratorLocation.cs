using H3.Model;
using H3;
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
                double latitude = 55.615 + _random.NextDouble() * (55.771 - 55.615);
                double longitude = 12.298 + _random.NextDouble() * (12.650 - 12.298);

                // Create a GeoJsonPoint for the Location
                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7);

                var mockGeolocation = new UserGeolocation
                {
                    UserId = $"mockUserId{i + 1}",
                    Location = location,
                    H3Index = h3Index.ToString()
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

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7);

                var mockGeolocation = new UserGeolocation
                {
                    UserId = $"AarhusUserId{i + 1}",
                    Location = location,
                    H3Index = h3Index.ToString()
                };

                mockData.Add(mockGeolocation);
            }

            return mockData;
        }

        public static List<UserGeolocation> GenerateMockDataForMon(int numberOfEntries)
        {
            var mockData = new List<UserGeolocation>();

            for (int i = 0; i < numberOfEntries; i++)
            {
                // Generate random coordinates within the specified range for Møn
                double latitude = 54.900 + _random.NextDouble() * (55.010 - 54.900);
                double longitude = 12.100 + _random.NextDouble() * (12.560 - 12.100);

                var location = GeoJson.Point(GeoJson.Geographic(longitude, latitude));

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7);

                var mockGeolocation = new UserGeolocation
                {
                    UserId = $"MonUserId{i + 1}",
                    Location = location,
                    H3Index = h3Index.ToString()
                };

                mockData.Add(mockGeolocation);
            }

            return mockData;
        }

        

    }
}
