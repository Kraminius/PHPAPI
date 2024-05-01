using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using PHPAPI.Model;

namespace PHPAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class StoreLocationController : ControllerBase
    {
        private readonly MongoStoreDBService _mongoDBService;
        private readonly IMongoCollection<Store> store;

        public StoreLocationController(MongoStoreDBService mongoDBService)
        {
            Console.WriteLine("I am in geolocationcontroller.");
            _mongoDBService = mongoDBService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Post([FromBody] Store geolocation)
        {
            await _mongoDBService.InsertGeolocationAsync(geolocation);
            return Ok("Geolocation inserted successfully.");
        }

        [HttpGet("findNearest")]
        public async Task<ActionResult<Store>> FindNearest(double latitude, double longitude, int meters, string brand)
        {
            try
            {
                var nearestStore = await _mongoDBService.FindNearestAsync(latitude, longitude, meters, brand);
                if (nearestStore != null)
                {
                    return Ok(nearestStore);
                }
                return NotFound("No nearby stores found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAllGeolocations()
        {
            try
            {
                await _mongoDBService.DeleteAllGeolocationsAsync();
                return Ok("All geolocations have been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting geolocations: " + ex.Message);
            }
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<Store>>> GetAllGeolocations()
        {
            try
            {
                var geolocations = await _mongoDBService.GetAllGeolocationsAsync();
                if (geolocations != null && geolocations.Count > 0)
                {
                    return Ok(geolocations);
                }
                return Ok("No geolocations found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving geolocations: " + ex.Message);
            }
        }

        // New API endpoint to insert mock data
        [HttpPost("insertMock")]
        public async Task<IActionResult> InsertMockStores()
        {
            try
            {
                await _mongoDBService.InsertMockDataIfNeededAsync();
                return Ok("Mock data inserted successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while inserting mock data: " + ex.Message);
            }
        }

        // Endpoint to generate and insert mock data for Copenhagen
        [HttpPost("generateMock/Copenhagen")]
        public async Task<IActionResult> GenerateMockStoreCopenhagen()
        {
            try
            {
                // Insert all mock geolocations into the database
                await store.InsertManyAsync(MockDataGeneratorLocation.GenerateMockStoresForCopenhagen(10, 2, 8));
                return Ok($"Mock stores for Copenhagen generated and inserted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inserting mock data: {ex.Message}");
                return StatusCode(500, "An error occurred while inserting mock data: " + ex.Message);
            }
        }

        // Endpoint to generate and insert mock data for Aarhus
        [HttpPost("generateMock/Aarhus")]
        public async Task<IActionResult> GenerateMockDataAarhus(int numberOfEntries)
        {
            var mockData = MockDataGeneratorLocation.GenerateMockDataForAarhus(numberOfEntries);
            await _mongoDBService.InsertManyGeolocationAsync(mockData);
            return Ok($"{numberOfEntries} mock entries for Aarhus generated and inserted successfully.");
        }

        // Endpoint to generate and insert mock data for Møn
        [HttpPost("generateMock/Mon")]
        public async Task<IActionResult> GenerateMockDataMon(int numberOfEntries)
        {
            var mockData = MockDataGeneratorLocation.GenerateMockDataForMon(numberOfEntries);
            await _mongoDBService.InsertManyGeolocationAsync(mockData);
            return Ok($"{numberOfEntries} mock entries for Møn generated and inserted successfully.");
        }

        [HttpGet("findByH3Index")]
        public async Task<ActionResult<List<Store>>> FindByH3Index(string h3Index)
        {
            try
            {
                var matchingGeolocations = await _mongoDBService.FindGeolocationsByH3IndexAsync(h3Index);
                if (matchingGeolocations != null && matchingGeolocations.Count > 0)
                {
                    return Ok(matchingGeolocations);
                }
                return NotFound("No geolocations found with the given H3 index.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving geolocations: " + ex.Message);
            }
        }

    }

    /*
    [Route("api/[controller]")]
    [ApiController]
    public class StoreLocationController : ControllerBase
    {
        private readonly MongoStoreDBService _mongoDBService;

        public StoreLocationController(MongoStoreDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Post([FromBody] Store geolocation)
        {
            await _mongoDBService.InsertGeolocationAsync(geolocation);
            return Ok("Geolocation inserted successfully.");
        }

        [HttpGet("findNearest")]

        public async Task<ActionResult<Store>> FindNearest(double latitude, double longitude, int meters, string brand)
        {
            try
            {
                var nearestStore = await _mongoDBService.FindNearestAsync(latitude, longitude, meters, brand);
                if (nearestStore != null)
                {
                    return Ok(nearestStore);
                }
                return NotFound("No nearby stores found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("deleteAll")]
        public async Task<IActionResult> DeleteAllGeolocations()
        {
            try
            {
                await _mongoDBService.DeleteAllGeolocationsAsync();
                return Ok("All geolocations have been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting geolocations: " + ex.Message);
            }
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<Store>>> GetAllGeolocations()
        {
            try
            {
                var geolocations = await _mongoDBService.GetAllGeolocationsAsync();
                if (geolocations != null && geolocations.Count > 0)
                {
                    return Ok(geolocations);
                }
                return NotFound("No geolocations found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving geolocations: " + ex.Message);
            }
        }

    }
    */
}