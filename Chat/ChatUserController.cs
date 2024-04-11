using Microsoft.AspNetCore.Mvc;
using PeopleHelpPeople.Model; // Assuming this is your namespace for models
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class ChatUserController : ControllerBase
{
    private readonly MongoDBService _mongoDBService;

    public ChatUserController(MongoDBService mongoDBService)
    {
        _mongoDBService = mongoDBService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ChatUser>> RegisterUser([FromBody] ChatUser user)
    {
        await _mongoDBService.AddUserAsync(user);
        return Ok(user);
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<ChatUser>> GetUser(string id)
    {
        var user = await _mongoDBService.FindUserByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}
