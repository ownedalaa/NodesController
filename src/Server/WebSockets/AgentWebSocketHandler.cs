using Newtonsoft.Json;
using Shared.Classes.Commands;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Server.Websockets;

public class AgentWebSocketHandler
{
    static ConcurrentDictionary<string, WebSocket> connectedAgents = new ConcurrentDictionary<string, WebSocket>();
    private static ConcurrentDictionary<string, TaskCompletionSource<string>> pendingRequests = new();
    public static async Task HandleAsync(HttpContext context)
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

        //__SendAsync(nodeId, "Hello from server!").Wait();

        // receive and close handling
        try
        {
            // keep the connection open until the client closes it
            // incoming messages are not processed since the server only sends commands to the agent
            // the only message we care about is the close message
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

                // process the incoming message and match it with the pending request
                var response = JsonConvert.DeserializeObject<CommandResponse>(text);
                Console.WriteLine(text);
                if (response != null && pendingRequests.TryRemove(response.RequestId, out var pending))
                    pending.TrySetResult(response.Payload ?? "");
            

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
                try
                {
                    await socket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "bye",
                        CancellationToken.None
                    );
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            socket.Dispose();
        }
    }


    public static List<string> GetConnectedAgents()
    {
        return connectedAgents.Keys.ToList();
    }

    public static bool IsAgentConnected(string nodeId)
    {
        return connectedAgents.ContainsKey(nodeId);
    }
    
    public static async Task<string> SendCommandAsync(string nodeId, string command, string? payload)
    {
        var requestId = Guid.NewGuid().ToString();

        var tcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);

        pendingRequests[requestId] = tcs;

        var message = JsonConvert.SerializeObject(new
        {
            requestId,
            command,
            payload
        });

        await __SendAsync(nodeId, message);

        try
        {
            return await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10));
        }
        finally
        {
            pendingRequests.TryRemove(requestId, out _);
        }
    }

    //priv
    private static async Task __SendAsync(string nodeId, string text)
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
