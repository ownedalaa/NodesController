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

    [HttpGet]
    public async Task<ActionResult<List<AgentNode>>> GetAgents()
    {
        return await _db.Agents.ToListAsync();
    }

    [HttpGet("{id}")]
    public IActionResult GetAgent(string id)
    {
        return Ok($"Agent: {id}");
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
            Secret = secret
        };

        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();

        return Ok(new
        {
            agent.NodeId,
            Secret = secret
        });
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