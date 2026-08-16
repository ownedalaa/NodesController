using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Classes.Commands;

public enum CommandType
{
    Print
}

public record Command
{
    public CommandType Type { get; set; }
    public string Payload { get; set; } = "";
}