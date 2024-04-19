using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using PHPAPI.Model;

namespace PHPAPI.Services
{
    public class InstantiateData()
    {

        public async Task SuperMarkets(int markets)
        {
            try
            {
                var mockGeolocations = new List<MarketGeolocation>();

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

    }
}