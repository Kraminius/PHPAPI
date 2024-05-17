using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;

        public RequestController(MongoDBService mongoDBService)
        {
            _mongoDBService = mongoDBService;
        }
        /*

        [HttpGet("matches")]
        public async Task<IActionResult> Get([FromBody] String Token, String UserID)
        {
            if (Token == null) { return BadRequest("Token is required."); }
            if (UserID == null) { return BadRequest("UserID is required."); }

            //TODO: Find users in same polygon(hash) as user requesting.
        }
        */
    }

}