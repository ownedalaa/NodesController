using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Server.Websockets;

public class AgentWebSocketHandler
{
    ConcurrentDictionary<string, WebSocket> connectedAgents = new ConcurrentDictionary<string, WebSocket>();

    public async Task HandleAsync(HttpContext context)
    {
        var nodeId = context.Request.RouteValues["nodeId"]?.ToString();
        
        if (string.IsNullOrWhiteSpace(nodeId))
        {
            context.Response.StatusCode = 400;
            return;
        }

        if (!context.WebSockets.IsWebSocketRequest)
        {
            context.Response.StatusCode = 400;
            return;
        }

        var socket = await context.WebSockets.AcceptWebSocketAsync();

        connectedAgents[nodeId] = socket;

        Console.WriteLine($"{nodeId} connected");

        var buffer = new byte[4096];

        SendAsync(nodeId, "Hello from server!").Wait();

        // receive and close handling
        try
        {
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    buffer,
                    CancellationToken.None
                );

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var text = Encoding.UTF8.GetString(
                    buffer,
                    0,
                    result.Count
                );

                Console.WriteLine($"[{nodeId}] {text}");
            }
        }
        catch (WebSocketException)
        {
            Console.WriteLine($"{nodeId} connection lost");
        }
        finally
        {
            connectedAgents.TryRemove(nodeId, out _);

            Console.WriteLine($"{nodeId} disconnected");

            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                await socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "bye",
                    CancellationToken.None
                );
            }

            socket.Dispose();
        }
    }

    private async Task SendAsync(string nodeId, string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);

        if (connectedAgents.TryGetValue(nodeId, out var socket))
        {
            await socket.SendAsync(
                    bytes,
            WebSocketMessageType.Text,
            true,
            CancellationToken.None
        );
        }
    }
}
