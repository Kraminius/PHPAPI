using Org.BouncyCastle.Utilities.Zlib;

namespace PHPAPI.Services
{
    using H3;
    using H3.Model;
    using Org.BouncyCastle.Utilities.Zlib;
    using System.Collections.Generic;

    public class H3Service
    {
        public List<string> GetH3Indexes(List<(double lat, double lon)> waypoints, int resolution)
        {
            var h3Indexes = new List<string>();

            foreach (var waypoint in waypoints)
            {
                var h3Index = H3Index.FromLatLng(new LatLng(waypoint.lat, waypoint.lon), resolution).ToString();
                h3Indexes.Add(h3Index.ToString());
            }

            return h3Indexes;
        }
    }

}
