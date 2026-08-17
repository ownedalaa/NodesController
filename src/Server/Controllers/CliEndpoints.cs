using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Websockets;
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
    public async Task<List<string>> GetOnlineAgents()
    {
        return AgentWebSocketHandler.GetConnectedAgents();
    }

    [HttpPost("execute")]
    public async Task<IActionResult> SendCommand(
        [FromForm] string nodeId,
        [FromForm] string command,
        [FromForm] string? payload)
    {
        if (!AgentWebSocketHandler.IsAgentConnected(nodeId))
            return NotFound("agent offline");

        var response =
            await AgentWebSocketHandler.SendCommandAsync(
                nodeId,
                command,
                payload
            );

        return Ok(response);
    }
}
