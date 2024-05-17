using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using H3.Model;
using H3;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;
using Azure.Core;
using NetTopologySuite.Index.HPRtree;
using MongoDB.Driver;
using System.Linq;
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
        public async Task<ActionResult<List<UserGeolocation>>> FindByH3Index(string requestId, string item, double longitude, double latitude)
        {
            try
            {
                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7).ToString();
                var matchingGeolocations = await _mongoDBService.FindGeolocationsByH3IndexAsync(h3Index);

                if (!matchingGeolocations.Any())
                    return NotFound("No geolocations found with the given H3 index.");

                var deliveryRequests = matchingGeolocations.Select(geo => new DeliveryRequest
                {
                    RequestId = requestId,
                    HelperId = geo.UserId,
                    Item = item,
                    Status = "Pending",
                    DeliveryLocationLatitude = latitude,
                    DeliveryLocationLongitude = longitude,
                    H3Index = h3Index
                }).ToList();

                await _mongoDBService.InsertManyRequests(deliveryRequests);
                return Ok(matchingGeolocations);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000) // Checking directly for the duplicate key error code
                    return Conflict("Duplicate entry detected. A delivery request with the same RequestId and HelperId combination already exists.");
                throw;
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving geolocations: " + ex.Message);
            }
        }






        [HttpGet("GetAllRequests")]
        public async Task<ActionResult<List<DeliveryRequest>>> GetAllRequests()
        {
            try
            {
                var requests = await _mongoDBService.DeliveryRequestsAsync();
                if (requests != null && requests.Count > 0)
                {
                    return Ok(requests);
                }
                return Ok("No requests found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving requests: " + ex.Message);
            }
        }



        [HttpGet("getMyH3")]
        public ActionResult<string> GetMyH3Index(double latitude, double longitude)
        {
            try
            {
                // Ensure latitude and longitude are within the valid range
                if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
                {
                    return BadRequest("Invalid latitude or longitude values. Ensure latitude is between -90 and 90, and longitude is between -180 and 180.");
                }

                var h3Index = H3Index.FromLatLng(new LatLng(latitude, longitude), 7); // Assuming the use of resolution 7

                if (h3Index == null)
                {
                    return NotFound("Unable to generate H3 index for the provided coordinates.");
                }

                return Ok(h3Index.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while calculating H3 index: {ex.Message}");
            }
        }

        [HttpDelete("deleteAllRequests")]
        public async Task<IActionResult> DeleteAllRequests()
        {
            try
            {
                await _mongoDBService.DeleteAllDeliveryRequestsAsync();
                return Ok("All delivery requests have been deleted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting delivery requests: " + ex.Message);
            }
        }


    }
}