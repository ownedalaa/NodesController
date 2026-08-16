using System.Net.WebSockets;
using System.Text;

string nodeId = "test-node-123";

using var socket = new ClientWebSocket();


await socket.ConnectAsync(
    new Uri($"ws://localhost:5082/ws/agent/{nodeId}"),
    CancellationToken.None
);

Console.WriteLine("Connected!");

var buffer = new byte[4096];

while (socket.State == WebSocketState.Open)
{
    var result = await socket.ReceiveAsync(
        buffer,
        CancellationToken.None
    );

    if (result.MessageType == WebSocketMessageType.Close)
    {
        Console.WriteLine("Server closed connection");
        break;
    }

    var message = Encoding.UTF8.GetString(
        buffer,
        0,
        result.Count
    );

    Console.WriteLine($"Server says: {message}");
}

SendAsync(socket, "Hello from client!").Wait();

static async Task SendAsync(
    ClientWebSocket socket,
    string text)
{
    var bytes = Encoding.UTF8.GetBytes(text);

    await socket.SendAsync(
        bytes,
        WebSocketMessageType.Text,
        true,
        CancellationToken.None
    );
}