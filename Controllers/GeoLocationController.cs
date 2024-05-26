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
using PHPAPI.Services;
using System.Security.Claims;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeolocationController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly IUserService _userService;

        public GeolocationController(MongoDBService mongoDBService, IUserService userService)
        {
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        /*
        [HttpPost("insert")]
        public async Task<IActionResult> Post([FromBody] UserGeolocation geolocation)
        {
            await _mongoDBService.InsertGeolocationAsync(geolocation);
            return Ok("Geolocation inserted successfully.");
        }
        */

        [HttpGet("findNearest")]
        [Authorize]
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

        /* [HttpGet("findByH3Index")]
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
        }  */

        [HttpPost("request")]
        [Authorize]
        public async Task<ActionResult<List<UserGeolocation>>> FindByH3Index([FromBody] DeliveryRequestInput requestInput)
        {
            try
            {
                // Log received requestInput
                Console.WriteLine("Received requestInput: " + JsonConvert.SerializeObject(requestInput));

                // Extract and log claims from the token
                var claims = User.Claims;
                foreach (var claim in claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                // Extract user ID from JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    Console.WriteLine("No NameIdentifier claim found.");
                    return Unauthorized("Invalid token.");
                }

                // Retrieve the user from the database
                var requestUser = await _userService.GetUserByUsernameAsync(userIdClaim);
                if (requestUser == null)
                {
                    Console.WriteLine("User not found in database.");
                    return NotFound("User not found.");
                }

                // Validate input request fields
                if (requestInput.Location == null)
                {
                    Console.WriteLine("RequestInput Location is null.");
                    return BadRequest("Location is required.");
                }
                if (requestInput.Wares == null || requestInput.Wares.Length == 0)
                {
                    Console.WriteLine("RequestInput Wares is null or empty.");
                    return BadRequest("Wares are required.");
                }

                // Compute the H3 index for the provided location
                
                var h3Index = H3Index.FromLatLng(new LatLng(requestInput.Location.X, requestInput.Location.Y), 7).ToString();
                Console.WriteLine("Computed H3 index: " + h3Index);

                // Find matching geolocations by H3 index
                var matchingGeolocations = await _mongoDBService.FindGeolocationsByH3IndexAsync(h3Index);
                if (matchingGeolocations == null || !matchingGeolocations.Any())
                {
                    Console.WriteLine("No geolocations found with the given H3 index.");
                    return NotFound("No geolocations found with the given H3 index.");
                }

                // Filter out the user's own geolocation
                var filteredGeolocations = matchingGeolocations.Where(geo => geo.Id != requestUser.Id).ToList();

                // Log each filtered geolocation
                foreach (var geo in filteredGeolocations)
                {
                    Console.WriteLine("Matching geolocation: " + JsonConvert.SerializeObject(geo));
                }

                // Create delivery requests for matching geolocations
                var deliveryRequests = filteredGeolocations.Select(geo =>
                {
                    return new DeliveryRequest
                    {
                        Id = ObjectId.GenerateNewId(),
                        RequestUser = requestUser,
                        HelpUser = new User { Id = geo.Id, Username = geo.Username, Email = geo.Email, Name = geo.Name, Location = geo.Location },
                        Wares = requestInput.Wares,
                        State = DeliveryRequest.StateOfRequest.Status.REQUESTED,
                        Location = requestInput.Location,
                        H3Index = h3Index,
                        TimeOfRequest = requestInput.TimeOfRequest
                    };
                }).ToList();

                // Log deliveryRequests
                foreach (var request in deliveryRequests)
                {
                    Console.WriteLine("Generated deliveryRequest: " + JsonConvert.SerializeObject(request));
                }

                // Insert the new delivery requests into the database
                await _mongoDBService.InsertManyRequests(deliveryRequests);

                return Ok(filteredGeolocations);
            }
            catch (MongoWriteException ex)
            {
                if (ex.WriteError.Code == 11000) // Check for duplicate key error code
                {
                    Console.WriteLine("Duplicate entry detected.");
                    return Conflict("Duplicate entry detected.");
                }
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving geolocations: " + ex);
                return StatusCode(500, "An error occurred while retrieving geolocations: " + ex.Message);
            }
        }


        [HttpGet("request")]
        [Authorize]
        public async Task<ActionResult<List<DeliveryRequest>>> GetRequests()
        {
            try
            {
                // Extract user ID from JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                {
                    Console.WriteLine("No NameIdentifier claim found.");
                    return Unauthorized("Invalid token.");
                }

                // Retrieve the user from the database
                var user = await _userService.GetUserByUsernameAsync(userIdClaim);
                if (user == null)
                {
                    Console.WriteLine("User not found in database.");
                    return NotFound("User not found.");
                }

                // Query the database for requests where the user is either the requester or the helper
                var filter = Builders<DeliveryRequest>.Filter.Or(
                    Builders<DeliveryRequest>.Filter.Eq(r => r.RequestUser.Id, user.Id),
                    Builders<DeliveryRequest>.Filter.Eq(r => r.HelpUser.Id, user.Id)
                );

                var requests = await _mongoDBService.DeliveryRequests.Find(filter).ToListAsync();

                if (requests == null || requests.Count == 0)
                {
                    return NotFound("No requests found.");
                }

                return Ok(requests);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving requests: " + ex);
                return StatusCode(500, "An error occurred while retrieving requests: " + ex.Message);
            }
        }

        HttpGet("request/{h3Index}")





        /*
        [HttpPost("assign")]
        [Authorize]
        public async Task<ActionResult> AssignRequest([FromBody] AssignRequestModel assignRequest)
        {
            try
            {
                // Extract user ID from JWT token
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                if (userIdClaim == null)
                    return Unauthorized("Invalid token.");

                // Retrieve the request from the database
                var filter = Builders<DeliveryRequest>.Filter.Eq(r => r.RequestIdKey, assignRequest.RequestIdKey);
                var update = Builders<DeliveryRequest>.Update
                    .Set(r => r.HelpUser)
                    .Set(r => r.State, DeliveryRequest.StateOfRequest.Status.ONGOING);

                // Update the matched request to ONGOING
                var result = await _mongoDBService.DeliveryRequests.UpdateOneAsync(
                    Builders<DeliveryRequest>.Filter.And(
                        filter,
                        Builders<DeliveryRequest>.Filter.Eq(r => r.HelpUser.Id, ObjectId.Parse(userIdClaim))),
                    update);

                if (result.MatchedCount == 0)
                    return NotFound("Request not found.");

                // Delete all other requests with the same RequestIdKey
                var deleteFilter = Builders<DeliveryRequest>.Filter.And(
                    filter,
                    Builders<DeliveryRequest>.Filter.Ne(r => r.HelpUser.Id, ObjectId.Parse(userIdClaim)));

                await _mongoDBService.DeliveryRequests.DeleteManyAsync(deleteFilter);

                return Ok("Request assigned successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while assigning the request: " + ex.Message);
            }
        }


        public class AssignRequestModel
        {
            public string RequestIdKey { get; set; }
        }
        */





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