using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Classes.Commands;

public record AgentCommand
{
    public string RequestId { get; set; } = "";
    public string Command { get; set; } = "";
    public string Payload { get; set; } = "";
}
public class CommandResponse
{
    public string RequestId { get; set; } = "";
    public bool Success { get; set; }
    public string? Payload { get; set; }
    public string? Error { get; set; }
}
