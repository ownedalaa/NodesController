using Microsoft.EntityFrameworkCore;
using Server.Websockets;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=app.db"));

// build
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Server API v1");
    });
}


//ws
app.UseWebSockets();

app.Map("/ws/agent/{nodeId}", async context =>
{
    var handler = context.RequestServices.GetRequiredService<AgentWebSocketHandler>();

    await handler.HandleAsync(context);
});


// apply migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
}

// the rest..
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
