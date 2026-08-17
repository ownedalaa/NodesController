using System.Net.Http;
using Shared.Models;

namespace ServerCli.Commands;

public enum ServerCommandType
{
    Exit
}

class CommandHandler
{
    private static readonly HttpClient client = new HttpClient();


    public static async Task<string> Execute(string[] command)
    {
        switch (command[0])
        {
            case "getagentinfo":
                if (command.Length == 4)
                {
                    var values = new Dictionary<string, string> {
                        { "nodeId", command[1] },
                        { "command", command[2] },
                        { "payload", command[3] }
                    };

                    var content = new FormUrlEncodedContent(values);
                    var response = await client.PostAsync($"https://localhost:7238/api/server/execute", content);

                    return response.Content.ReadAsStringAsync().Result;
                }

                return "Command incomplete";

            case "servers":
                return client.GetStringAsync("https://localhost:7238/api/server/online").Result;

            case "help":
                return "Available commands: help, exit, clear, servers";

            case "exit":
                return "bye";
            
            case "clear":
                return "";

            default:
                return "Command not found";
        }
    }
}