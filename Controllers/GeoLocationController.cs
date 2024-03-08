using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model;

[Route("api/[controller]")]
[ApiController]
public class GeolocationController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;

    public GeolocationController(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserGeolocation geolocation)
    {
        await _mongoDBService.InsertGeolocationAsync(geolocation);
        return Ok("Geolocation inserted successfully.");
    }
}