using System.Text;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

//using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shared.Models;
using System.Diagnostics;

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
            //case "execute":
            //    if (command.Length == 4 || command.Length == 3)
            //    {
            //        var values = GetPayload(command);


            //        var response = __Post(values).Result;

            //        return response.Content.ReadAsStringAsync().Result;
            //    }

            //    return "Incorrect syntax: [execute nodeID command payload]";

            case "ping":
                var timer = new Stopwatch();
                
                timer.Start();
                string response = __Execute(command);
                timer.Stop();

                return (response != "agent offline") ? $"{response} - RTT: {timer.ElapsedMilliseconds}ms)" : response;

               

            case "servers":
                var result = client.GetStringAsync("https://localhost:7238/api/server/online").Result;

                List<string>? lista = JsonSerializer.Deserialize<List<string>>(result);
                StringBuilder sb = new StringBuilder();

                foreach (var item in lista)
                {
                    sb.Append(item.ToString() + " ");
                }

                return (sb.ToString() is null) ? "" : sb.ToString();

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

    private static async Task<HttpResponseMessage> __Post(Dictionary<string, string> payload)
    {
        var content = new FormUrlEncodedContent(payload);
        var response = await client.PostAsync($"https://localhost:7238/api/server/execute", content);

        return response;
    }

    private static Dictionary<string, string> __GetPayload(string[] command)
        => new Dictionary<string, string> {
                        { "nodeId", command[1] },
                        { "command", command[0] },
                        { "payload", (command.Length == 3) ? command[2] : ""}
                    };
    private static string __Execute(string[] command)
    {
        if (command.Length == 3 || command.Length == 2)
        {
            var values = __GetPayload(command);
            return __Post(values).Result.Content.ReadAsStringAsync().Result;
        }

        return $"Incorrect syntax: [{command[0]} nodeID payload]";
    }
}