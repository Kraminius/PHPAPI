using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeopleHelpPeople.Controllers
{
    [Route("api/[controller]")] // Defines the route template
    [ApiController] // Signifies that this controller responds to web API requests
    public class SearchMatchesController : ControllerBase // Change the name to follow convention and inherit from ControllerBase
    {
        [HttpGet(Name = "GetMatch")] // Indicates that this action handles GET requests
        public ActionResult<IEnumerable<Model.Match>> GetAllMatches()
        {
            // Assuming DataLayer.GetAllMatches() fetches data from the data source
            var matches = DataLayer.GetAllMatches();
            if (matches == null) return NotFound(); // Returns a 404 if no matches are found
            return Ok(matches); // Returns a 200 OK response with the matches
        }
    }
}