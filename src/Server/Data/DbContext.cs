using Microsoft.EntityFrameworkCore;
using Shared.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AgentNode> Agents => Set<AgentNode>();
}