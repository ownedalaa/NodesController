using System.Text;
using System.Net.Http;
using System.Text.Json;

using Shared.Models;
using Shared.Classes.Commands;

namespace Agent.Commands;

class CommandHandler
{
    private static readonly HttpClient client = new HttpClient();


    public static async Task<string> Execute(AgentCommand ac)
    {
        switch (ac.Command)
        {
            case "ping":
                return "Pong!";

            //case "":
            //    return "";

            default:
                return "Command not found";
        }
    }
}
