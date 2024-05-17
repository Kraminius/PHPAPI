using Microsoft.AspNetCore.Mvc;
using PHPAPI.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PHPAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchMatchesController : ControllerBase
    {
        [HttpGet(Name = "GetMatch")]
        public ActionResult<IEnumerable<Model.Match>> GetAllMatches()
        {
            var matches = DataLayer.GetAllMatches();
            if (matches == null) return Ok("No matches found..."); // Returns a 200 with message if no matches are found
            return Ok(matches); // Returns a 200 OK response with the matches
        }
    }
}