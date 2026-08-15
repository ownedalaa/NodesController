using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.Security.Cryptography;
using System.Text;

namespace Server.Controllers;

[ApiController]
[Route("api/server")]
public class ServerController : ControllerBase
{
    private AppDbContext _db;

    public ServerController(AppDbContext db)
    {
        _db = db;
    }

    //[HttpGet("getagents")]
    //public async Task<ActionResult<List<AgentNode>>> GetAgents()
    //{
    //    return await _db.Agents.ToListAsync();
    //}

    [HttpGet("online")]
    public async Task<IActionResult> GetOnlineAgents()
    {
        var cutoff = DateTime.UtcNow.AddSeconds(-30);

        var agents = await _db.Agents
            .Where(a => a.LastSeen >= cutoff)
            .Select(a => new
            {
                a.NodeId,
                a.Name,
                a.LastSeen
            })
            .ToListAsync();

        return Ok(agents);
    }
}
