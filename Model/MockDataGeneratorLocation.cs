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

    }
}
