using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GeolocationController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public GeolocationController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }

        [HttpPost("insert")]
        public async Task<IActionResult> Post([FromBody] UserGeolocation geolocation)
        {
            await _mongoDBService.InsertGeolocationAsync(geolocation);
            return Ok("Geolocation inserted successfully.");
        }

        [HttpGet("findNearest")]

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
    }
}