namespace PHPAPI.Services
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using PHPAPI.Model.Geospatial;

    using System.Net.Http;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;
    using System.Collections.Generic;

    public class OsrmService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public async Task<List<(double lat, double lon)>> GetRoute(string start, string end)
        {
            var url = $"http://router.project-osrm.org/route/v1/driving/{start};{end}?overview=full&geometries=polyline&steps=false&annotations=false";
            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(responseBody);
            var route = json["routes"][0]["geometry"].ToString();

            return DecodePolyline(route);
        }

        private List<(double lat, double lon)> DecodePolyline(string polyline)
        {
            var points = new List<(double lat, double lon)>();
            var index = 0;
            var len = polyline.Length;
            var lat = 0;
            var lng = 0;

            while (index < len)
            {
                int b, shift = 0, result = 0;
                do
                {
                    b = polyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlat = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = polyline[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                } while (b >= 0x20);

                int dlng = ((result & 1) != 0 ? ~(result >> 1) : (result >> 1));
                lng += dlng;

                points.Add((lat / 1E5, lng / 1E5));
            }

            return points;
        }
    }


}
