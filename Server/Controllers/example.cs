using Microsoft.AspNetCore.Mvc;

namespace Server.Controllers;


[ApiController]
[Route("api/agents")]
public class AgentsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAgents()
    {
        return Ok("agents go here");
    }

    [HttpGet("{id}")]
    public IActionResult GetAgent(string id)
    {
        return Ok($"Agent: {id}");
    }

    [HttpPost("register")]
    public IActionResult RegisterAgent()
    {
        return Ok("registered");
    }
}