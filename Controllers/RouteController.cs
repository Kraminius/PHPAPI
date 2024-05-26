

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using PHPAPI.Services;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RouteController : ControllerBase
    {
        private readonly OsrmService _osrmService;
        private readonly H3Service _h3Service;

        public RouteController(OsrmService osrmService, H3Service h3Service)
        {
            _osrmService = osrmService;
            _h3Service = h3Service;
        }

        [HttpGet("get-h3-route")]
        public async Task<IActionResult> GetH3Route([FromQuery] string start, [FromQuery] string end, [FromQuery] int resolution = 9)
        {
            var waypoints = await _osrmService.GetRoute(start, end);
            var h3Indexes = _h3Service.GetH3Indexes(waypoints, resolution);

            Console.WriteLine("H3 Indexes: " + h3Indexes);

            return Ok(h3Indexes);
        }
    }
}