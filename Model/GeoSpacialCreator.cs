using H3;
using H3.Algorithms;
using H3.Extensions;
using H3.Model;
using System;
using System.Collections.Generic;

namespace PHPAPI.Model
{
    public class GeoSpacialCreator
    {

        public static void Main(string[] args)
        {
            // Create an instance of GeoSpacialCreator
            GeoSpacialCreator creator = new GeoSpacialCreator();

            // Call the CreateHexagons method
            creator.CreateHexagons();
        }

        public void CreateHexagons()
        {
            var centerLatitude = 55.6761;  // Example latitude
            var centerLongitude = 12.5683; // Example longitude
            var resolution = 8;            // Example resolution

            // Create a H3 index from latitude and longitude
            var h3Index = H3Index.FromLatLng(new LatLng(centerLatitude, centerLongitude), resolution);


            // Get all hexagons within k distance
            var hexagons = h3Index.GridDiskDistances(10);  // 10 is just an example distance

            foreach (var hex in hexagons)
            {
                var hexIndex = hex.Index;
                var hexLatLng = hexIndex.ToLatLng(); // Corrected method invocation

                Console.WriteLine($"Hexagon Index: {hexIndex}, Latitude: {hexLatLng.Latitude}, Longitude: {hexLatLng.Longitude}");
            }
        }

    }
}
