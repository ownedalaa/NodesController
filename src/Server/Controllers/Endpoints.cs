using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Server.Controllers;

[ApiController]
[Route("api/agents")]
public class AgentsController : ControllerBase
{
    private AppDbContext _db;

    public AgentsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<Agent>>> GetAgents()
    {
        return await _db.Agents.ToListAsync();
    }

    [HttpGet("{id}")]
    public IActionResult GetAgent(string id)
    {
        return Ok($"Agent: {id}");
    }

    [HttpPost("register")]
    public async Task<IActionResult> AddAgent(Agent agent)
    {
        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();

        return Ok(agent);
    }
}