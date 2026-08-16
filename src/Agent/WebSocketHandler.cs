using Shared.Classes.Commands;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using Newtonsoft.Json;

namespace Agent
{
    class WebSocketHandler
    {
        public static async Task HandleConnection(string nodeId)
        {
            using var socket = new ClientWebSocket();


            await socket.ConnectAsync(
                new Uri($"ws://localhost:5082/ws/agent/{nodeId}"),
                CancellationToken.None
            );

            Console.WriteLine("Connected!");

            var buffer = new byte[4096];

            //SendAsync(socket, "Hello from client!").Wait();

            // receive
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

                //Console.WriteLine($"Server says: {message}");
                var command = JsonConvert.DeserializeObject<AgentCommand>(message);

                if (command == null)
                    return;

                string responsePayload = "temp";

                var response = JsonConvert.SerializeObject(new
                {
                    requestId = command.RequestId,
                    success = true,
                    payload = responsePayload
                });

                await SendAsync(socket, response);
            }
        }

        private static async Task SendAsync(
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
    }
}
