using Microsoft.AspNetCore.Http.Headers;
using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Import the namespace where your models are located
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PeopleHelpPeople.Controllers
{
    [Route("api/[controller]")] // Defines the route template
    [ApiController] // Signifies that this controller responds to web API requests
    public class ChatController : ControllerBase // Change the name to follow convention and inherit from ControllerBase
    {
        [HttpGet(Name = "Chat")] // Indicates that this action handles GET requests
        public ActionResult GetMessage([FromHeader] int id)
        {
            //ActionResult<IEnumerable<Model.Chat>> GetChat();

            // Assuming DataLayer.getChat fetches data from the data source
            var chat = DataLayer.GetChat(id);
            if (chat == null) return NotFound(); // Returns a 404 if no chats are found
            return Ok(chat); // Returns a 200 OK response with the chat
        }
    }


}