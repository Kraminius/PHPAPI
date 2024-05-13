using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H3.Model;
using H3;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;
using Microsoft.AspNetCore.Authorization;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeolocationController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public GeolocationController(MongoDBService mongoDBService)
        {
            Console.WriteLine("I am in geolocationcontroller.");
            _mongoDBService = mongoDBService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Post([FromBody] UserGeolocation geolocation)
        {
            await _mongoDBService.InsertGeolocationAsync(geolocation);
            return Ok("Geolocation inserted successfully.");
        }

        [HttpGet("findNearest")]
        //[Authorize] TODO: ENABLE AUTHORIZE
        public async Task<ActionResult<UserGeolocation>> FindNearest(double latitude, double longitude, int meters)
        {
            try
            {
                var nearestUser = await _mongoDBService.FindNearestAsync(latitude, longitude, meters);
                if (nearestUser != null)
                {
                    return Ok(nearestUser);
                }
                return NotFound("No nearby user found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }



        [HttpDelete("deleteAll")]
        [Authorize]
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
        public async Task<ActionResult<List<UserGeolocation>>> GetAllGeolocations()
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
        public async Task<IActionResult> InsertMockData()
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
        public async Task<IActionResult> GenerateMockDataCopenhagen(int numberOfEntries)
        {
            var mockData = MockDataGeneratorLocation.GenerateMockDataCopenhagen(numberOfEntries);
            await _mongoDBService.InsertManyGeolocationAsync(mockData);
            return Ok($"{numberOfEntries} mock entries for Copenhagen generated and inserted successfully.");
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
        public async Task<ActionResult<List<UserGeolocation>>> FindByH3Index(string h3Index)
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
}