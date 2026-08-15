using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using System.Security.Cryptography;
using System.Text;

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

    [HttpPost("register")]
    public async Task<IActionResult> Register(string name)
    {
        var secretBytes = RandomNumberGenerator.GetBytes(32);
        var secret = Convert.ToBase64String(secretBytes);

        var agent = new AgentNode
        {
            NodeId = Guid.NewGuid().ToString(),
            Name = name,
            Secret = secret,
            LastSeen = DateTime.Now,
        };

        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            agent.NodeId,
            Secret = secret
        });
    }


    [HttpPost("heartbeat")]
    public async Task<IActionResult> Heartbeat(string nodeId, string secret)
    {
        var agent = await _db.Agents
            .FirstOrDefaultAsync(a => a.NodeId == nodeId);

        if (agent is null)
            return NotFound();

        if (agent.Secret != secret)
            return Unauthorized();

        agent.LastSeen = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        return Ok();
    }
}
//public static class HashHelper
//{
//    public static string ComputeSha256Hash(string input)
//    {
//        using (SHA256 sha256 = SHA256.Create())
//        {
//            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
//            // Convert byte array to hex string
//            StringBuilder builder = new StringBuilder();
//            for (int i = 0; i < bytes.Length; i++)
//            {
//                builder.Append(bytes[i].ToString("x2"));
//            }
//            return builder.ToString();
//        }
//    }
//}